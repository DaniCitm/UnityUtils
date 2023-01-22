///
/// Simple pooling for Unity.
///   Author: Martin "quill18" Glaude (quill18@quill18.com)
///   Extended: Simon "Draugor" Wagner (https://www.twitter.com/Draugor_/)
///   Extended: Daniel Castaño Estrella (daniel.c.estrella@gmail.com)
///   Latest Version: https://gist.github.com/danielcestrella/b1b45999da9cec063f9c381e296693a8
///   License: CC0 (http://creativecommons.org/publicdomain/zero/1.0/)
///   UPDATES:
///		2019-07-15 by Daniel Castaño Estrella:	- Change Stack to Queue to avoid reuse last used prefab (useful with active despawned GameObjects);
///		2019-01-24 by Daniel Castaño Estrella:	- Added possibility of delayed Despawn;
///												- Added SimplePool parent to keep Hierarchy clean.
///		2018-09-27 by Daniel Castaño Estrella:	- Added possibility to Despawn without deactivate. Useful to leave a particle effect or decal for a while but be able to reusit if needed.
///												- Added possibility of lifeTime to Spawned GameObjects.
///     2018-01-04 by Simon "Draugor" Wagner:	- Added Extension Method for Despawn on GameObjects 
///												- Changed the Member Lookup so it doesn't require a PoolMemberComponent anymore.
///													- for that i added a HashSet which contains all PoolMemberIDs  (HashSet has O(1) contains operator)
///													- PoolMemberIDs are just ints from GameObject.getInstanceID() which are unique for the GameObject 
///														over the runtime of the game
///												- Changed PoolDictionary from (Prefab, Pool) to (int, Pool) using Prefab.GetInstanceID
/// 	2015-04-16 by Martin "quill18" Glaude:	- Changed Pool to use a Stack generic.
/// 
/// Usage:
/// 
///   There's no need to do any special setup of any kind.
/// 
///   Instead of calling Instantiate(), use this:
///       SimplePool.Spawn(somePrefab, somePosition, someRotation);
/// 
///   Instead of destroying an object, use this:
///       SimplePool.Despawn(myGameObject);
///   or this:
///       myGameObject.Despawn();
/// 
///   If desired, you can preload the pool with a number of instances:
///       SimplePool.Preload(somePrefab, 20);
/// 
/// Remember that Awake and Start will only ever be called on the first instantiation
/// and that member variables won't be reset automatically.  You should reset your
/// object yourself after calling Spawn().  (i.e. You'll have to do things like set
/// the object's HPs to max, reset animation states, etc...)
/// 
/// 
/// 

using UnityEngine;
using System.Collections.Generic;

public static class SimplePool
{
	private static SimplePoolCoroutinesHandler coroutinesMB = null;
	private static SimplePoolCoroutinesHandler CoroutinesMB
	{
		get
		{
			if (!coroutinesMB)
			{
				GameObject g = new GameObject("SimplePoolCoroutinesHandler");
				Object.DontDestroyOnLoad(g);
				coroutinesMB = g.AddComponent<SimplePoolCoroutinesHandler>();
			}
			return coroutinesMB;
		}
	}
	// You can avoid resizing of the Queue's internal data by
	// setting this to a number equal to or greater to what you
	// expect most of your pool sizes to be.
	// Note, you can also use Preload() to set the initial size
	// of a pool -- this can be handy if only some of your pools
	// are going to be exceptionally large (for example, your bullets.)
	public const int DEFAULT_POOL_SIZE = 3;

	private class SimplePoolCoroutinesHandler : MonoBehaviour { }

	/// <summary>
	/// The Pool class represents the pool for a particular prefab.
	/// </summary>
	public class Pool
	{
		private GameObject simplePoolParent = null;
		public GameObject SimplePoolParent
		{
			get
			{
				if (simplePoolParent == null)
				{
					simplePoolParent = GameObject.Find("SimplePool");
					if (simplePoolParent == null) simplePoolParent = new GameObject("SimplePool");
				}
				return simplePoolParent;
			}
		}
		// We append an id to the name of anything we instantiate.
		// This is purely cosmetic.
		private int _nextId = 1;
		// The structure containing our inactive objects.
		// Why a Queue and not a List? Because we'll never need to
		// pluck an object from the start or middle of the array.
		// We'll always just grab the last one, which eliminates
		// any need to shuffle the objects around in memory.
		private readonly Queue<GameObject> _inactive;
		//A Hashset which contains all GetInstanceIDs from the instantiated GameObjects 
		//so we know which GameObject is a member of this pool.
		public readonly HashSet<int> MemberIDs;
		// The prefab that we are pooling
		private readonly GameObject _prefab;

		// Constructor
		public Pool(GameObject prefab, int initialQty)
		{
			_prefab = prefab;
			// If Queue uses a linked list internally, then this
			// whole initialQty thing is a placebo that we could
			// strip out for more minimal code. But it can't *hurt*.
			_inactive = new Queue<GameObject>(initialQty);
			MemberIDs = new HashSet<int>();
		}

		// Spawn an object from our pool
		public GameObject Spawn(Vector3 pos, Quaternion rot, float lifeTime = 0, bool deactivateOnDespawn = true)
		{
			GameObject obj;
			if (_inactive.Count == 0)
			{
				//#if UNITY_EDITOR
				//				if(Time.time > 1) Debug.LogWarning("Instantiated new: " + _prefab, _prefab);	//Time.time > 1 to not count initial preloads
				//#endif
				// We don't have an object in our pool, so we
				// instantiate a whole new object.
				obj = GameObject.Instantiate<GameObject>(_prefab, pos, rot);
				obj.name = _prefab.name + " (" + (_nextId++) + ")";

				// Add the unique GameObject ID to our MemberHashset so we know this GO belongs to us.
				MemberIDs.Add(obj.GetInstanceID());
			}
			else
			{
				// Grab the last object in the inactive array
				obj = _inactive.Dequeue();

				if (obj == null)
				{
					// The inactive object we expected to find no longer exists.
					// The most likely causes are:
					//   - Someone calling Destroy() on our object
					//   - A scene change (which will destroy all our objects).
					//     NOTE: This could be prevented with a DontDestroyOnLoad
					//	   if you really don't want this.
					// No worries -- we'll just try the next one in our sequence.

					return Spawn(pos, rot);
				}
			}

			obj.transform.position = pos;
			obj.transform.rotation = rot;
			obj.SetActive(true);

			if (lifeTime != 0)
			{
				CoroutinesMB.StartCoroutine(DespawnCoroutine(obj, lifeTime, deactivateOnDespawn));
			}

			obj.transform.parent = SimplePoolParent.transform;

			return obj;
		}

		private System.Collections.IEnumerator DespawnCoroutine(GameObject gameObject, float delay, bool deactivate)
		{
			yield return new WaitForSeconds(delay);

			Despawn(gameObject, deactivate);
		}

		public void Despawn(GameObject obj, float delay, bool deactivate = true)
		{
			CoroutinesMB.StartCoroutine(DespawnCoroutine(obj, delay, deactivate));
		}

		// Return an object to the inactive pool.
		public void Despawn(GameObject obj, bool deactivate = true)
		{
			obj.SetActive(!deactivate);

			// Since Queue doesn't have a Capacity member, we can't control
			// the growth factor if it does have to expand an internal array.
			// On the other hand, it might simply be using a linked list 
			// internally.  But then, why does it allow us to specify a size
			// in the constructor? Maybe it's a placebo? Queue is weird.
			_inactive.Enqueue(obj);
		}

	}

	// All of our pools
	public static Dictionary<int, Pool> _pools;

	/// <summary>
	/// Initialize our dictionary.
	/// </summary>
	private static void Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
	{
		if (_pools == null)
			_pools = new Dictionary<int, Pool>();

		if (prefab != null)
		{
			//changed from (prefab, Pool) to (int, Pool) which should be faster if we have 
			//many different prefabs.
			var prefabID = prefab.GetInstanceID();
			if (!_pools.ContainsKey(prefabID))
				_pools[prefabID] = new Pool(prefab, qty);
		}
	}

	/// <summary>
	/// If you want to preload a few copies of an object at the start
	/// of a scene, you can use this. Really not needed unless you're
	/// going to go from zero instances to 100+ very quickly.
	/// Could technically be optimized more, but in practice the
	/// Spawn/Despawn sequence is going to be pretty darn quick and
	/// this avoids code duplication.
	/// </summary>
	static public void Preload(GameObject prefab, int qty = 1)
	{
		Init(prefab, qty);
		// Make an array to grab the objects we're about to pre-spawn.
		var obs = new GameObject[qty];
		for (int i = 0; i < qty; i++)
			obs[i] = Spawn(prefab, Vector3.zero, Quaternion.identity);

		// Now despawn them all.
		for (int i = 0; i < qty; i++)
			Despawn(obs[i]);
	}

	/// <summary>
	/// Spawns a copy of the specified prefab (instantiating one if required).
	/// NOTE: Remember that Awake() or Start() will only run on the very first
	/// spawn and that member variables won't get reset.  OnEnable will run
	/// after spawning -- but remember that toggling IsActive will also
	/// call that function.
	/// </summary>
	static public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, float lifeTime = 0, bool deactivateOnDespawn = true)
	{
		Init(prefab);

		//#if DEBUG	//for development build and UNITY_EDITOR
		//		Debug.Log("SimplePool Spawn: " + prefab + " with lifeTime: " + lifeTime + " and deactivateOnDespawn: " + deactivateOnDespawn);
		//#endif

		return _pools[prefab.GetInstanceID()].Spawn(pos, rot, lifeTime, deactivateOnDespawn);
	}

	/// <summary>
	/// Despawn the specified gameobject back into its pool.
	/// </summary>
	static public void Despawn(GameObject obj, bool deactivate = true)
	{
		Pool p = null;
		foreach (var pool in _pools.Values)
		{
			if (pool.MemberIDs.Contains(obj.GetInstanceID()))
			{
				p = pool;
				break;
			}
		}

		if (p == null)
		{
			Debug.Log("Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead.");
			GameObject.Destroy(obj);
		}
		else
		{
			p.Despawn(obj, deactivate);
		}

		//#if DEBUG  //for development build and UNITY_EDITOR
		//		Debug.Log("SimplePool Despawn: " + obj + " with deactivate: " + deactivate);
		//#endif
	}

	static public void Despawn(GameObject obj, float delay, bool deactivate = true)
	{
		Pool p = null;
		foreach (var pool in _pools.Values)
		{
			if (pool.MemberIDs.Contains(obj.GetInstanceID()))
			{
				p = pool;
				break;
			}
		}

		if (p == null)
		{
			Debug.Log("Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead.");
			GameObject.Destroy(obj, delay);
		}
		else
		{
			p.Despawn(obj, delay, deactivate);
		}

		//#if DEBUG  //for development build and UNITY_EDITOR
		//		Debug.Log("SimplePool Despawn: " + obj + " with delay: " + delay + " and deactivate: " + deactivate);
		//#endif
	}
}

public static class SimplePoolGameObjectExtensions
{
	public static void Despawn(this GameObject go, bool deactivate = true)
	{
		SimplePool.Despawn(go, deactivate);
	}

	public static void Despawn(this GameObject go, float delay, bool deactivate = true)
	{
		SimplePool.Despawn(go, delay, deactivate);
	}
}
