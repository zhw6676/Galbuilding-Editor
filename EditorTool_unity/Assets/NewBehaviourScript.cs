using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class NewBehaviourScript : MonoBehaviour {
	public GameObject Tips;
	public GameObject SaveObstacleBTN;
	public GameObject SaveMonsterBTN;
	public GameObject SavePlayerBTN;
	public GameObject LoadObstacleBTN;
	public GameObject LoadMonsterBTN;
	public GameObject LoadPlayerBTN;
	public GameObject ClearAll;

	public GameObject ObstacleBlock;
	public GameObject MonsterBlock;
	public GameObject PlayerBlock;
	public Image CurrentChoosenBolck;

	private string CurrentChoosen;
	private bool isFill;
	private bool isClear;
	private Color ObstacleColor = new Color(240F/255F, 86F/255F, 140F/255F, 255F/255F);
	private Color MonsterColor = new Color(38F/255F, 244F/255F, 64F/255F, 255F/255F);
	private Color PlayerColor = new Color(65F/255F, 167F/255F, 227F/255F, 255F/255F);
	private List<Vector2> ObstacleList = new List<Vector2> ();
	private List<Vector2> MonsterList = new List<Vector2> ();
	private List<Vector2> PlayerList = new List<Vector2> ();

	private Text texTip;
	void Start () {
		CurrentChoosen = "ObstacleBlock";

		EventTriggerListener.Get (SaveObstacleBTN).onClick += onClickHandler;
		EventTriggerListener.Get (SaveMonsterBTN).onClick += onClickHandler;
		EventTriggerListener.Get (SavePlayerBTN).onClick += onClickHandler;
		EventTriggerListener.Get (LoadObstacleBTN).onClick += onClickHandler;
		EventTriggerListener.Get (LoadMonsterBTN).onClick += onClickHandler;
		EventTriggerListener.Get (LoadPlayerBTN).onClick += onClickHandler;

		EventTriggerListener.Get (ObstacleBlock).onClick += onClickHandler;
		EventTriggerListener.Get (MonsterBlock).onClick += onClickHandler;
		EventTriggerListener.Get (PlayerBlock).onClick += onClickHandler;
		EventTriggerListener.Get (ClearAll).onClick += onClickHandler;

		texTip = Tips.transform.Find ("Tips").GetComponent<Text> ();
		EventTriggerListener.Get (texTip.gameObject).onClick += onClickHandler;

		for( int i = 0; i < 900; i++ ){
			GameObject obj = GameObject.Find ("Image (" + i + ")");
			EventTriggerListener.Get (obj).onClick += onClickHandler;
			EventTriggerListener.Get (obj).onEnter += onHoverHandler;
		}
	}

	private void onHoverHandler(GameObject OBJ){
		if( isFill || isClear){
			SetChoosen (OBJ);
		}
	}

	private void onClickHandler(GameObject OBJ){
		switch(OBJ.name){
		case "SaveObstacleBTN":
		case "SaveMonsterBTN":
		case "SavePlayerBTN":
			StartCoroutine(SaveContent ());
			break;
		case "LoadObstacleBTN":
		case "LoadMonsterBTN":
		case "LoadPlayerBTN":
			StartCoroutine(LoadContent (OBJ.name));
			break;
		case "ClearAll":
			ClearAllData ();
			break;
		case "ObstacleBlock":
			CurrentChoosen = OBJ.name;
			CurrentChoosenBolck.color = ObstacleColor;
			break;
		case "MonsterBlock":
			CurrentChoosen = OBJ.name;
			CurrentChoosenBolck.color = MonsterColor;
			break;
		case "PlayerBlock":
			CurrentChoosen = OBJ.name;
			CurrentChoosenBolck.color = PlayerColor;
			break;
		case "Tips":
			Tips.SetActive (false);
			break;
		default:
			SetChoosen (OBJ);
			break;
		}
	}

	private void ClearAllData(){
		for( int i = 0; i < 900; i++ ){
			GameObject obj = GameObject.Find ("Image (" + i + ")");
			obj.GetComponent<Image> ().color = Color.white;
		}

		ObstacleList = new List<Vector2> ();
		MonsterList = new List<Vector2> ();
		PlayerList = new List<Vector2> ();
	}

	private void SetChoosen(GameObject OBJ){
		int length = OBJ.name.Length - 8;
		int id = int.Parse(OBJ.name.Substring (7,length));
		Vector2 coord = new Vector2 (id / 30, id % 30);
		Image img = OBJ.GetComponent<Image> ();

		if( isClear ){
			/// 清除色块
			if( img.color == ObstacleColor ){
				img.color = Color.white;
				ObstacleList.Remove (coord);
			}else if( img.color == MonsterColor ){
				img.color = Color.white;
				MonsterList.Remove (coord);
			}else if( img.color == PlayerColor ){
				img.color = Color.white;
				PlayerList.Remove (coord);
			}
		}else if( isFill ){
			/// 填充色块
			switch(CurrentChoosen){
			case "ObstacleBlock":
				img.color = ObstacleColor;
				if(!ObstacleList.Contains(coord)){
					ObstacleList.Add (coord);
				}
				break;
			case "MonsterBlock":
				img.color = MonsterColor;
				if(!MonsterList.Contains(coord)){
					MonsterList.Add (coord);
				}
				break;
			case "PlayerBlock":
				img.color = PlayerColor;
				if(!PlayerList.Contains(coord)){
					PlayerList.Add (coord);
				}
				break;
			}
		}else{
			/// 单选/反选
			if( img.color == Color.white ){
				switch(CurrentChoosen){
				case "ObstacleBlock":
					img.color = ObstacleColor;
					ObstacleList.Add (coord);
					break;
				case "MonsterBlock":
					img.color = MonsterColor;
					MonsterList.Add (coord);
					break;
				case "PlayerBlock":
					img.color = PlayerColor;
					PlayerList.Add (coord);
					break;
				}
			}else{
				if( img.color == ObstacleColor ){
					ObstacleList.Remove (coord);
				}else if( img.color == MonsterColor ){
					MonsterList.Remove (coord);
				}else if( img.color == PlayerColor ){
					PlayerList.Remove (coord);
				}
				img.color = Color.white;
			}
		}
	}

	void Update () {
		if( Input.GetMouseButtonDown(1) ){
			isFill = true;
		}
		if( Input.GetMouseButtonUp(1) ){
			isFill = false;
			isClear = false;
		}
		if( Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl)){
			isClear = true;
		}
	}

	/// <summary>
	/// 保存信息
	/// </summary>
	/// <returns>The content.</returns>
	IEnumerator SaveContent (){
		string context = "";
		List<Vector2> TempList = new List<Vector2>();
		switch(CurrentChoosen){
		case "ObstacleBlock":
			TempList = ObstacleList;
			break;
		case "MonsterBlock":
			TempList = MonsterList;
			break;
		case "PlayerBlock":
			TempList = PlayerList;
			break;
		}

		/// 解析信息内容
		for( int i = 0; i < TempList.Count; i++ ){
			context += TempList [i].x + "," + TempList [i].y;
			if(i != TempList.Count - 1){
				context += "|";
			}
		}

		TextEditor textEditor = new TextEditor ();
		textEditor.text = context;
		textEditor.OnFocus ();
		textEditor.Copy ();

		Tips.SetActive (true);

		yield return null;
	}

	/// <summary>
	/// 保存信息
	/// </summary>
	/// <returns>The content.</returns>
	IEnumerator LoadContent (string name){
		switch(name){
		case "LoadObstacleBTN":
			ObstacleList = new List<Vector2> ();
			break;
		case "LoadMonsterBTN":
			MonsterList = new List<Vector2> ();
			break;
		case "LoadPlayerBTN":
			PlayerList = new List<Vector2> ();
			break;
		}
		string context = "";
		TextEditor textEditor = new TextEditor ();
		textEditor.OnFocus ();
		textEditor.Paste ();
		context = textEditor.text;

		/// 解析文本内容
		char[] sign = new char[]{ '|' };
		string[] split = context.Split (sign);
		sign = new char[]{ ',' };
		for( int i = 0; i< split.Length; i++ ){
			string[] Breaks = split [i].Split (sign);
			if( Breaks.Length <= 1 ){
				if( context.Length > 50 ){
					context = context.Substring (0, 50);
				}
				texTip.text = "导入的数据格式有错误\n\n" +
					"<color=#FF0000>【" + context + " ...】</color>\n\n" +
					"点击关闭";
				Tips.SetActive (true);
				break;
			}
			Vector2 point = new Vector2 (int.Parse(Breaks [0]), int.Parse(Breaks [1]));
			float index = point.x * 30 + point.y;
			Image img = GameObject.Find("Image (" + index + ")").GetComponent<Image>();
			switch(name){
			case "LoadObstacleBTN":
				ObstacleList.Add (point);
				img.color = ObstacleColor;
				break;
			case "LoadMonsterBTN":
				MonsterList.Add (point);
				img.color = MonsterColor;
				break;
			case "LoadPlayerBTN":
				PlayerList.Add (point);
				img.color = PlayerColor;
				break;
			}
		}

		yield return null;
	}
}
