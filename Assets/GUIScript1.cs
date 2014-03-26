using UnityEngine;
using System.Collections;

public class GUIScript1 : MonoBehaviour {
	public Texture mud;
	bool canMove = true;
	public GameObject player;
	bool allyTurn = true;
	//bool enemyChoice = false;
	bool moving, power,missed, victory, loss, attacking, damaging, e_attacking, e_damaging, e_missed;
	bool end_played= false;
	int strr = 20;
	int dex = 15;
	int cha = 11;
	int inte = 12;
	int wis = 9;
	int movespeed = 20;
	int d20;
	int hit;
	int ed20;
	int ehit;
	int bab = 2;
	GameObject target;
	int targetAc = 13;
	int targetHealth = 50;
	int myAc = 13;
	int myHealth = 50;
	int dam;
	int edam;
	private float delay = 1.5f;
	public GameObject mon;
	private Animation anim;
	public AudioClip win;
	public AudioClip lose;
	public AudioClip roar;
	public AudioClip miss;
	public AudioClip damage;
	public AudioClip slap_o;

	// Use this for initialization
	void Start () {
		anim = mon.GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
		if (targetHealth <=0){
			victory = true;
			StartCoroutine(Victory());
		}
		if (myHealth <=0){
			loss = true;
			victory = false;
			Debug.Log("Game End");
			if(!end_played){
				AudioSource.PlayClipAtPoint(lose, transform.position);
				end_played = true;
			}
			gameObject.AddComponent("Rigidbody");
		}
		if(victory && mon.transform.position.y > 1000){
			mon.transform.position = new Vector3(mon.transform.position.x, mon.transform.position.y - .01f ,mon.transform.position.z);
		}

	}

	void OnGUI(){
		GUI.DrawTexture(new Rect(0,Screen.height-150,2000,250), mud);
		if (victory){
			GUI.Label (new Rect(20,Screen.height-140, 300, 30),
			           "The enemy has fallen! You are victorious!");
			allyTurn = false;
			if (GUI.Button(new Rect(20, Screen.height-110, 200, 30), "Play Again?")){
				Application.LoadLevel("tiny_cave");
			}
		}else if (loss){
			GUI.Label (new Rect(20,Screen.height-140, 300, 30),
			           "You have fallen. The enemy is victorious!");
			allyTurn = false;
			if (GUI.Button(new Rect(20, Screen.height-110, 200, 30), "Retry?")){
				Application.LoadLevel("tiny_cave");
			}
		}else if (allyTurn){

			//if(moving){
			//	GUI.Label (new Rect(20,Screen.height-140, 300, 30),
			//	           "You are moving!");
			//	StartCoroutine(Moving());
			//}else
			if(attacking){
				GUI.Label (new Rect(20,Screen.height-140, 300, 30),
				           "You roll " + d20 + ", and strike for " + hit + ".");
			}else if (missed){
				GUI.Label (new Rect(20,Screen.height-140, 300, 30),
				           "You missed.");
			}else if (damaging){
				GUI.Label (new Rect(20,Screen.height-140, 300, 30),
				           "You hit! You roll " + d20 + " and deal " + dam + ".");
			}else if(allyTurn){
				GUI.Label (new Rect(20,Screen.height-140, 450, 30),
					"Your health is " + myHealth + ", the enemy has " + targetHealth +" health. Choose an action:");

				//if (canMove){
				//	if (GUI.Button(new Rect(20, Screen.height-120, 200, 30), "Move")){
				//		moving = true;
				//	}
				//}

				if (GUI.Button(new Rect(20, Screen.height-110, 200, 30), "Power Attack")){
					AudioSource.PlayClipAtPoint(slap_o, transform.position);
					power = true;
					attacking = true;
					StartCoroutine(attackingI());
				}
				if (GUI.Button(new Rect(20, Screen.height-70, 200, 30), "Attack")){
					AudioSource.PlayClipAtPoint(slap_o, transform.position);
					attacking = true;
					StartCoroutine(attackingI());
				}
			}
		}else{
			if(e_attacking){

				GUI.Label (new Rect(20,Screen.height-140, 300, 30),
				           "The enemy rolls " + ed20 + ", and strikes at you for " + ehit + ".");
			}else if (e_missed){
				GUI.Label (new Rect(20,Screen.height-140, 300, 30),
				           "The enemy missed.");
			}else if (e_damaging){
				GUI.Label (new Rect(20,Screen.height-140, 300, 70),
				           "You are hit! The enemy rolls " + ed20 + " and deals " + edam + " to you. Your health  is down to " + myHealth + "." );
			}else{
				GUI.Label (new Rect(20,Screen.height-140, 200, 30),
			           "The enemy is choosing an action.");
			}
		}

	}

	IEnumerator wait(bool x){
		x = false;
		yield return new WaitForSeconds(delay);

	}
	IEnumerator wait(bool x, bool y){
		x = false;
		y = false;
		yield return new WaitForSeconds(delay);

	}

	//IEnumerator Moving(){
	//	yield return new WaitForSeconds(delay);
	//	moving = false;
	//}

	IEnumerator attackingI(){
		Debug.Log("Attacking");
		d20 = Random.Range(1,20);
		hit = (strr-10)/2 + d20 + bab;
		if (power){
			hit = hit -1;
		}
		if (targetAc < hit){
			yield return new WaitForSeconds(delay);
			damaging = true;
			StartCoroutine(damagingI());
		}else{
			yield return new WaitForSeconds(delay);
			attacking = false;
			missed = true;
			AudioSource.PlayClipAtPoint(miss, transform.position);
			yield return new WaitForSeconds(delay);
			power = false;
			missed = false;
			allyTurn = false;
			StartCoroutine(EnemyChoice());
		}
		attacking =false;
	}

	IEnumerator damagingI(){
		anim.Play("gothit");
		d20 = Random.Range(1,20);
		dam = (strr-10)/2 + d20 + bab;

		if (power){
			dam = dam + 2;
		}
		Debug.Log ("Dam" + dam);

		targetHealth = targetHealth - dam;
		AudioSource.PlayClipAtPoint(damage, transform.position);
		yield return new WaitForSeconds(delay);
		power = false;
		damaging = false;
		allyTurn = false;
		//anim.Blend("idle", 1f,.3f);
		anim.Play("idle");
		StartCoroutine(EnemyChoice());
	}

	IEnumerator EnemyAttacking(){
		if (!end_played){
			anim.Play("bitchslap");
			AudioSource.PlayClipAtPoint(slap_o, transform.position);
			Debug.Log("E Attacking");
			ed20 = Random.Range(1,20);
			ehit = (strr-10)/2 + ed20 + bab;
			if (power){
				ehit = ehit -1;
			}
			if (myAc < ehit){
				AudioSource.PlayClipAtPoint(miss, transform.position);
				yield return new WaitForSeconds(delay + 3f);
				anim.Play("idle");
				e_damaging = true;
				StartCoroutine(EnemyDamaging());
			}else{
				yield return new WaitForSeconds(delay);
				anim.Play("idle");
				e_attacking = false;
				e_missed = true;
				yield return new WaitForSeconds(delay);
				power = false;
				e_missed = false;
				allyTurn = true;
				}
			e_attacking =false;

		}
	}

	IEnumerator EnemyDamaging(){
		if (!end_played){
			ed20 = Random.Range(1,20);
			edam = (strr-10)/2 + ed20 + bab;

			if (power){
				edam = edam + 2;
			}
			Debug.Log ("Dam" + edam);
			
			myHealth = myHealth - edam;
			anim.Play("Roar");
			AudioSource.PlayClipAtPoint(roar, transform.position);
			yield return new WaitForSeconds(delay);
			power = false;
			e_damaging = false;
			allyTurn = true;
			anim.Play("idle");
		}
	}

	IEnumerator EnemyChoice(){
		if (!end_played){
			Debug.Log("Enemy choosing");
			int r = Random.Range(0,1);
			if (r == 0){
				power = true;
			}
			yield return new WaitForSeconds(delay);
			e_attacking = true;
			StartCoroutine(EnemyAttacking());
		}
	}

	IEnumerator Victory(){
		if (!end_played){

			AudioSource.PlayClipAtPoint(win, transform.position);
			Debug.Log("Game End");
			end_played = true;
			anim.Play("fall");
			yield return new WaitForSeconds(delay);
			//mon.SetActive(false);
			}
	}

}
