using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using game_core;

/// <summary>
/// Finger event.
/// </summary>
public class FingerEvent
{
	public List<ITouchable> touchedObjects=new List<ITouchable>();

	/// <summary>
	/// Initializes a new instance of the <see cref="FingerEvent"/> class.
	/// </summary>
	/// <param name="it">It.</param>
	public FingerEvent(ITouchable it=null)
	{
			if(it!=null)
			{
					Add (it);
			}	
	}
	/// <summary>
	/// Add the specified obj.
	/// </summary>
	/// <param name="obj">Object.</param>
	public bool Add(ITouchable obj)
	{
		if (!containsObject (obj)) 
		{	
				touchedObjects.Add (obj);
				return true;
		}
		return false;
	}

	/// <summary>
	/// Gets the last GameObject added.
	/// </summary>
	/// <returns>The last.</returns>
	public ITouchable getLast()
	{
			return touchedObjects.Last ();
	}
	/// <summary>
	/// Clears the list.
	/// </summary>
	public void clearList()
	{
			touchedObjects.Clear ();
	}

	/// <summary>
		/// Checks if touched gameObject(obj) was registered previously
	/// touched.
	/// </summary>
	/// <returns><c>true</c>, if object was containsed, <c>false</c> otherwise.</returns>
	/// <param name="obj">Object.</param>
	public bool containsObject(ITouchable obj)
	{
			
			foreach(ITouchable it in touchedObjects)
			{
					if(it.Equals(obj))
					{
							return true;
					}	
			}
			return false;
	}

	/// <summary>
	/// Remove the specified obj.
	/// </summary>
	/// <param name="obj">Object.</param>
	public ITouchable remove(ITouchable obj)
	{
			return touchedObjects.Find(x=>x.Equals(obj));
	}
}
/// <summary>
/// Multi touch controller.
/// </summary>
public class MultiTouchController : MonoBehaviour {
	//LAYER MASK NAME FOR EXAMPLE INPUT
	public 	LayerMask touchInputMask;

	//RAYCASTHIT IS THE STRUCTURE USED TO 
	//GET INFORMATION BACK FROM A RAYCAST...
	private RaycastHit 					hit;
	private RaycastHit[] 				hitList;
	private FingerEvent 				fingerEvent;
	private Dictionary<int,FingerEvent>	_touchTable = 	new Dictionary<int,FingerEvent> ();
    private GameObject clickRecipient = null;
	//...AND WHAT IS RAYCAST? Well,  
	//Raycast  is used to tell you what objects in the environment the ray 
	//runs into. (http://answers.unity3d.com/questions/53013/what-is-raycast-use-of-it.html) thanks!!!
	//In this case is used to know if some "INPUT" has been touched/Clicked.

	/// <summary>
	/// This function is called every fixed framerate frame.
	/// </summary>
	void Update () 
	{
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                bool rayFlag = Physics.Raycast(ray, out hit, touchInputMask);

                //FingerEvent		iTouchable;
                //IF NOT EXISTS ADD TO LIST
                if (!_touchTable.ContainsKey(touch.fingerId))
                {
                    _touchTable.Add(touch.fingerId, new FingerEvent());
                }
                if (_touchTable.TryGetValue(touch.fingerId, out fingerEvent))
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (rayFlag)
                            {
                                if (fingerEvent.Add(hit.transform.GetComponent<ITouchable>()))
                                {
                                    fingerEvent.getLast().OnTouchBegan(touch.position);
                                }
                            }
                            break;
                        case TouchPhase.Moved:
                            if (rayFlag)
                            {
                                foreach (ITouchable it in fingerEvent.touchedObjects)
                                {
                                    if (!it.Equals(hit.transform.GetComponent<ITouchable>()))
                                    {
                                        it.OnTouchEnded(touch.position);
                                    }
                                }

                                fingerEvent.touchedObjects.RemoveAll(item => item != hit.transform.GetComponent<ITouchable>());
                                if (fingerEvent.Add(hit.transform.GetComponent<ITouchable>()))
                                {
                                    fingerEvent.getLast().OnTouchBegan(touch.position);
                                }
                            } else {
                                if (fingerEvent.touchedObjects.Count > 0)
                                {
                                    foreach (ITouchable it in fingerEvent.touchedObjects) {
                                        //IT MUST BE MOVED AND THEN CALL ENDED INSIDE THE GAMEOBJECT REMEMBER THIS
                                        it.OnTouchEnded(touch.position);
                                    }
                                    fingerEvent.clearList();
                                }
                            }
                            break;
                        case TouchPhase.Stationary:
                            if (!rayFlag)
                            {
                                if (fingerEvent.touchedObjects.Count > 0)
                                {
                                    foreach (ITouchable it in fingerEvent.touchedObjects)
                                    {
                                        it.OnTouchEnded(touch.position);
                                    }
                                    fingerEvent.clearList();
                                }

                            }
                            break;
                        case TouchPhase.Canceled:
                        case TouchPhase.Ended:

                            if (fingerEvent.touchedObjects.Count > 0)
                            {
                                foreach (ITouchable it in fingerEvent.touchedObjects)
                                {
                                    it.OnTouchEnded(touch.position);
                                }
                            }
                            _touchTable.Remove(touch.fingerId);
                            break;
                    }
                }

            }
        } else if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, touchInputMask))
            {
                clickRecipient = hit.transform.gameObject;
                if (Input.GetMouseButtonDown(0))
                {

                    clickRecipient.SendMessage("OnTouchBegan", Input.mousePosition, SendMessageOptions.DontRequireReceiver);
                }
            }

        } else if (Input.GetMouseButtonUp(0) && clickRecipient!=null){
          clickRecipient.SendMessage("OnTouchEnded", Input.mousePosition, SendMessageOptions.DontRequireReceiver);
          clickRecipient = null;
        }else {

            if (_touchTable.Count > 0)
            {
                foreach (ITouchable it in _touchTable.Values)
                {
                    it.OnTouchEnded(Vector3.zero);
                }
                _touchTable.Clear();
            }
        }

		
	}
}
