using UnityEngine;
using System.Collections;
namespace game_core
{
/// <summary>
/// I touchable.
/// </summary>
public interface ITouchable
{
		void OnTouchBegan 		(Vector3 v);
		void OnTouchCanceled	(Vector3 v);
		void OnTouchEnded 		(Vector3 v);
		void OnTouchMoved 		(Vector3 v);		
}
/// <summary>
/// Touch behaviour
/// </summary>
public class TouchBehaviour : MonoBehaviour,ITouchable 
{
	
	/// <summary>
	/// The touch sem.
	/// </summary>
	protected bool touchSemaphore	=	true;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public virtual void Awake(){}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public virtual void OnEnable()
	{
		touchSemaphore	=	true;
	}
	/// <summary>
	/// Use this for initialization
	/// </summary>
	public virtual void Start () {}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public virtual void Update () {}
	
	//MOUSE TRIGGERs
	/// <summary>
	/// Raises the touch began event.
	/// </summary>
	/// <param name="v">V.</param>
	public virtual void OnTouchBegan(Vector3 v)		{}
	/// <summary>
	/// Raises the touch canceled event.
	/// </summary>
	/// <param name="v">V.</param>
	public virtual void OnTouchCanceled(Vector3 v)	{}
	/// <summary>
	/// Raises the touch ended event.
	/// </summary>
	/// <param name="v">V.</param>
	public virtual void OnTouchEnded(Vector3 v)		{ touchSemaphore=!touchSemaphore;}
	/// <summary>
	/// Raises the touch moved event.
	/// </summary>
	/// <param name="v">V.</param>
	public virtual void OnTouchMoved(Vector3 v)		{}
	/// <summary>
	/// Raises the touch stay event.
	/// </summary>
	/// <param name="v">V.</param>
	public virtual void OnTouchStay(Vector3 v)		{}
	public virtual void action()					{}
}
}