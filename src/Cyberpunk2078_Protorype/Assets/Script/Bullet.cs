
using System.Collections;

using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float bullet_speed = 50f;	
	private Transform heading;
	public Vector3 defaultPos;
	private bool refine_angle = false;

	private Vector3 direction;
	public bool Boom;
	[SerializeField] private GameObject boom;
	[SerializeField] private GameObject reflected;
	private SpriteRenderer m_render;
	private Vector3 toleranced_position;

	public int damage;
	// Use this for initialization
	void Start () {
		m_render = gameObject.GetComponent<SpriteRenderer>();
		refine_angle = false;
		defaultPos = transform.position;
	}

	void Update()
	{
		if (direction.x == 0 && direction.y == 0)
		{
			Destroy(gameObject);
		}
		transform.position += direction * bullet_speed * Time.deltaTime;
		if (!refine_angle)
		{
				float angleBoardRotZ = Mathf.Rad2Deg *
				                       Mathf.Atan(Mathf.Abs(transform.position.y - toleranced_position.y) /
				                                  Mathf.Abs(transform.position.x - toleranced_position.x));
				float constant = 1;
				if (transform.position.x > toleranced_position.x)
				{
					constant *= -1;
					//bullet is on the right side of target 
				}

				if (transform.position.y > toleranced_position.y)
				{
					//bullet is on the downside of target
					constant *= -1;
				}

				if (!float.IsNaN(angleBoardRotZ))
				{
					transform.Rotate(0, 0, constant * angleBoardRotZ);
				}
				refine_angle = true;
		}
		if (Mathf.Abs(transform.position.x) > 12.59||Mathf.Abs(transform.position.y) > 8.38)
		{
			Destroy(gameObject);
		}
		//Vector3 direction = heading.position - transform.position;
		// Move our position a step closer to the target.
//		direction = Vector3.RotateTowards(transform.right,direction, step,0.0f);
//		transform.rotation = Quaternion.LookRotation(direction);
	}

	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.transform.tag == "Ground"){
					Destroy(gameObject);
					GameObject Boom = Instantiate(boom, transform.position, transform.rotation);
					Destroy(Boom, 0.2f);
		}else if (col.transform.tag == "Shield" || col.transform.tag == "PlayerAtk" ){
				CameraManager._instance.Shake(0.03f,0.03f);
				Color myColor = new Color();
				ColorUtility.TryParseHtmlString("#FFFFFF", out myColor);
				GameObject Boom = Instantiate(reflected, transform.position, transform.rotation);
				Destroy(Boom, 0.2f);
				m_render.color = myColor;
				bullet_speed = 10;
				direction = -direction;
				GetComponent<SpriteRenderer>().flipX = true;
				transform.tag = "Bullet";
		}
			
		else if (col.transform.tag == "EnemyShield" && transform.tag == "Bullet"){
				Destroy(gameObject);
				gameObject.GetComponent<BoxCollider2D>().enabled = false;
				GameObject Boom = Instantiate(boom, transform.position, transform.rotation);
				Destroy(Boom, 0.2f);
		}
		
	}
	
	public void setDirection(Vector3 _direction,float offset = 0)
	{
		toleranced_position = _direction;
		direction = (toleranced_position - transform.position).normalized;
		if (offset != 0)
		{
			direction = rotate(direction, offset);
			toleranced_position = direction * 200;
		}
		if (toleranced_position.x - transform.position.x < 0)
		{
			transform.localScale = new Vector3(transform.localScale.x * -1,transform.localScale.y,transform.localScale.z); 
		}
	}

	public Vector3 rotate(Vector3 v,float degrees)
	{
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}

	IEnumerator Fadedelay(GameObject Boom){
		yield return new WaitForSeconds(0.25f);
	}
}