using UnityEngine;
using System.Collections;

public class AI3	 : MonoBehaviour
{
	//	public Camera mainCamera;
	public GameObject cube;
	public GameObject whitePrefab;
	public GameObject blackPrefab;
	public GUIText blackScoreText;
	public GUIText whiteScoreText;
	public GUIText turnText;
	//public GUIText firstTurnText;
	public GUIText winText;
	private const int boardSize = 8;
	private int[,] board = new int[8, 8];
	private int emptyPiece = 0;
	private int blackPiece = 1;
	private int whitePiece = 2;
	private int turn;
	private int search;
	private int whiteScore = 2; //default value
	private int blackScore = 2; //default value
	
	
	
	// Use this for initialization
	void Start ()
	{
		Initialize ();
		Draw ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		MouseClick ();
	}
	
	//Initialize the board
	void Initialize ()
	{
		for (int x = 0; x<boardSize; x++) {
			for (int y = 0; y<boardSize; y++) {
				board [x, y] = emptyPiece;
			}
		}
		board [3, 3] = board [4, 4] = blackPiece;
		board [3, 4] = board [4, 3] = whitePiece;
		
		turn = blackPiece;
		

		
	}
	
	//draw board
	void Draw ()
	{
		for (int x = 0; x<boardSize; x++) {
			
			for (int y = 0; y<boardSize; y++) {
				
				GameObject drawBoard = Instantiate 
					(cube, new Vector3 (x * 1.1f, y * 1.1f, 0), Quaternion.identity) as GameObject;
				drawBoard.transform.parent = transform; //this way to draw a board is temporary
				
				if (board [x, y] == emptyPiece)
					continue;
				
				Put (x, y);
			}
		}
	}
	
	void MouseClick ()
	{
		if (Input.GetButtonDown ("Fire1")) {
			Vector3 screenPoint = Input.mousePosition;
			screenPoint.z = 10;
			
			Vector3 v = Camera.main.ScreenToWorldPoint (screenPoint);
			int pieceX = (int)Mathf.Floor (v.x);
			int pieceY = (int)Mathf.Floor (v.y);
			
			if (!OnBoard (pieceX, pieceY)) // if clicked piece not in the board area, then return
				return;
			
			if (board [pieceX, pieceY] != emptyPiece)
				return;
			
			FlipCheck (pieceX, pieceY, turn);	
			GameOverCheck (turn);  //must check the gameover after click event is over, otherwise if there is no adaptive position to put, it would can not check the game over
		}
	}
	
	void FlipCheck (int x, int y, int color)
	{
		
		if (//check 8 directions
		    U (x, y, color) ||
		    D (x, y, color) ||
		    L (x, y, color) ||
		    R (x, y, color) ||
		    UL (x, y, color) || 
		    UR (x, y, color) || 
		    DL (x, y, color) || 
		    DR (x, y, color)
		    ) {
			if (U (x, y, color)) {
				//				Put2(pieceX,pieceY);  //the reason why i place calling here is that if this calling does not include in this function, then it can be called every mouse click time !
				flipU (x, y, color);
			}
			
			if (D (x, y, color)) {
				flipD (x, y, color);
			}
			
			if (L (x, y, color)) {
				flipL (x, y, color);
			}
			
			if (R (x, y, color)) {
				flipR (x, y, color);
			}
			
			if (UL (x, y, color)) {
				flipUL (x, y, color);
			}
			
			if (UR (x, y, color)) {
				flipUR (x, y, color);
			}
			
			if (DL (x, y, color)) {
				flipDL (x, y, color);
			}
			
			if (DR (x, y, color)) {
				flipDR (x, y, color);
			}
			
			Put2 (x, y);
			TurnChange ();	
			ScoreCount ();
			
			
		} else
			return;
	}
	
	void GameOverCheck (int color)
	{
		if (!CanPut (blackPiece) && !CanPut (whitePiece)) {  // only if black & white both can not be found to put on the board, the game should be over
			
			if (whiteScore > blackScore) {
				//Debug.Log("White win");
				winText.text = "白方勝利! ";
			} else if (whiteScore < blackScore) {
				//Debug.Log("Black win");
				winText.text = "黒方勝利! ";
			} else
				winText.text = "引き分け! ";
			
		} else if (!CanPut (color)) {  // if one of the pieces can be found among 2 types, it should change the turn for another player
			TurnChange ();                                            // process: put black->turn is white->check game over->check if white does not have a way to put->change the turn  
		}
		
		return;
	}
	
	public bool CanPut (int color)
	{
		
		for (int x=0; x < boardSize; x++) {
			for (int y = 0; y < boardSize; y++) {
				if (board [x, y] == emptyPiece) {
					if (!(U (x, y, color) ||
					      D (x, y, color) ||
					      L (x, y, color) ||
					      R (x, y, color) ||
					      UL (x, y, color) || 
					      UR (x, y, color) || 
					      DL (x, y, color) || 
					      DR (x, y, color))) {
						continue;
					} else
						return true;
				}
				//else if(board[x, y] != emptyPiece){
				
				
				//continue;}
			}
		}
		Debug.Log ("can not put the " + color + " piece any more");
		return false;
		
	}
	
	//put the pieces to the board for beginning status
	void Put (int x, int y)
	{
		if (board [x, y] == whitePiece) {
			
			GameObject _whitePrefab = Instantiate 
				(whitePrefab, new Vector3 (x * 1.1f, y * 1.1f, 0), Quaternion.identity) as GameObject;
			_whitePrefab.transform.parent = transform;
		} else if (board [x, y] == blackPiece) {
			
			GameObject _blackPrefab = Instantiate 
				(blackPrefab, new Vector3 (x * 1.1f, y * 1.1f, 0), Quaternion.identity) as GameObject;
			_blackPrefab.transform.parent = transform;
		}
	}
	
	//put the pieces based on the turn, and assign the color information to board
	void Put2 (int x, int y)
	{
		if (turn == whitePiece) {
			
			GameObject _whitePrefab = Instantiate 
				(whitePrefab, new Vector3 (x * 1.1f, y * 1.1f, 0), Quaternion.identity) as GameObject;
			_whitePrefab.transform.parent = transform;
			board [x, y] = whitePiece;
		} else if (turn == blackPiece) {
			
			GameObject _blackPrefab = Instantiate 
				(blackPrefab, new Vector3 (x * 1.1f, y * 1.1f, 0), Quaternion.identity) as GameObject;
			_blackPrefab.transform.parent = transform;
			board [x, y] = blackPiece;
		}
	}
	
	
	/* 8 functions for checking 8 direction 
	 */
	
	public bool U (int x, int y, int color)     //up
	{
		
		search = 1;
		y++;
		
		//search
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					y++;
					search++;
				}
			} else
				return false;
			
			//when you use "while", must notice this , do not loop unlimitedly, otherwaise
			//computer would be done....
		}
		return false;
	}
	
	public bool D (int x, int y, int color) // down
	{
		
		search = 1;
		y--;
		
		//search
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					y--;
					search++;
				}
			} else
				return false;
			
		}
		return false;
	}
	
	public bool L (int x, int y, int color) //left
	{
		
		search = 1;
		x--;
		//search
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					x--;
					search++;
				}
			} else
				return false;
			
		}
		return false;
	}
	
	public bool R (int x, int y, int color)  //right
	{
		
		search = 1;
		x++;
		//search
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					x++;
					search++;
				}
			} else
				return false;
			
		}
		return false;
	}
	
	public bool UL (int x, int y, int color) //up left
	{
		
		search = 1;
		x--;
		y++;
		
		//search
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					x--;
					y++;
					search++;
				}
			} else
				return false;
			
		}
		return false;
	}
	
	public bool UR (int x, int y, int color) //up right
	{
		
		search = 1;
		x++;
		y++;
		
		//search
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					x++;
					y++;
					search++;
				}
			} else
				return false;				
			
		}
		return false;
	}
	
	public bool DL (int x, int y, int color) // down left
	{
		
		search = 1;
		x--;
		y--;
		
		//search
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					x--;
					y--;
					search++;
				}
			} else
				return false;
			
		}
		return false;
	}
	
	public bool DR (int x, int y, int color) //down right
	{
		
		search = 1;
		x++;
		y--;
		
		while (OnBoard(x,y)) { 
			if (board [x, y] != emptyPiece) {
				
				if (board [x, y] == color) {
					
					if (search > 1) {  // search
						return true;
					} else {
						return false;
					}
				} else {
					
					x++;
					y--;
					search++;
				}
			} else
				return false;
			
		}
		return false;
	}
	
	/* 8 functions for 8 directions to do reversi work   
when flip pieces notice that do not change the turn, because this is just *flip*
	 */ 
	void flipU (int x, int y, int color)
	{
		y++;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				y++;
			} else
				break;
		}
		
		
	}
	
	void flipD (int x, int y, int color)
	{
		y--;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				y--;
			} else
				break;
		}
	}
	
	void flipL (int x, int y, int color)
	{
		x--;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				x--;
			} else
				break;
		}
	}
	
	void flipR (int x, int y, int color)
	{
		x++;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				x++;
			} else
				break;
		}
	}
	
	void flipUL (int x, int y, int color)
	{
		x--;
		y++;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				x--;
				y++;
			} else
				break;
		}
	}
	
	void flipUR (int x, int y, int color)
	{
		x++;
		y++;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				x++;
				y++;
			} else
				break;
		}
	}
	
	void flipDL (int x, int y, int color)
	{
		x--;
		y--;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				x--;
				y--;
			} else
				break;
		}
	}
	
	void flipDR (int x, int y, int color)
	{
		x++;
		y--;
		
		while (OnBoard(x, y)) {
			
			if (board [x, y] != color) {  //if the color is not the same as previous piece
				Put2 (x, y);
				x++;
				y--;
			} else
				break;
		}
	}
	
	/// ///////////////////
	
	void TurnChange ()
	{
		if (turn == blackPiece) {
			
			turn = whitePiece;
			turnText.text = "石の出番：白石";
			//Debug.Log ("White !");
			
		} else {
			turn = blackPiece;
			//Debug.Log ("black !");
			turnText.text = "石の出番：黒石";
		}
		
	}
	
	//check clicked position if located on the board area
	public bool OnBoard (int x, int y)
	{
		if (x < 0 || x > 7)
			return false;
		if (y < 0 || y > 7)
			return false;
		return true;
	}
	
	void ScoreCount ()  // search the board(8*8) and count the pieces by colors
	{
	
		whiteScore = 0;
		blackScore = 0;
		
		for (int x=0; x < boardSize; x++) {
			for (int y = 0; y < boardSize; y++) {
				if (board [x, y] == blackPiece) {
					blackScore ++;
				} else if (board [x, y] == whitePiece) {  //when you use "else if" , once the first "if" is true, would not check the "else if" below 
					whiteScore ++;
				} else 
					continue;
				
			}
		}
		
	
		blackScoreText.text = "黒石の数： " + blackScore.ToString (); 
		whiteScoreText.text = "白石の数： " + whiteScore.ToString (); 
	}
	

	
}










