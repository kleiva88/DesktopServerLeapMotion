﻿
/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class HandControlMobile : MonoBehaviour {

	// receiving Thread
	Thread receiveThread;

	// udpclient object
	UdpClient client;

	private int port = 1999; 

	string[] sArray = new string[16];

	public GameObject rightHandPrefab,leftHandPrefab;

	private GameObject currentRightHand, currentLeftHand;

	private bool shouldUpdateHands = false;
	private bool shouldDestroyHands = false;

	private bool rightHandExists = false;
	private bool leftHandExists = false;

	private Vector3 position1, position2;
	private Quaternion rotation1, rotation2;


	// start from unity3d
	public void Start()
	{
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
	}

	void Update(){

		if (shouldDestroyHands) {
			shouldDestroyHands = false;
			if (transform.childCount > 0) {
				DestroyHands ();
			}
		}

		if (shouldUpdateHands) {
			shouldUpdateHands = false;
			UpdateHands ();
		}
	}

	void UpdateHands(){

		rightHandExists = false;
		leftHandExists = false;

		if (sArray.Length > 1) {
			sArray[1] = sArray [1].TrimStart ('(');
			sArray [3] = sArray [3].TrimEnd (')');
			sArray [4] = sArray [4].TrimStart ('(');
			sArray [7] = sArray [7].TrimEnd (')');
	

			position1 = new Vector3(float.Parse(sArray[1]),float.Parse(sArray[2]),float.Parse(sArray[3]));
			rotation1 = new Quaternion(float.Parse(sArray[4]),float.Parse(sArray[5]),float.Parse(sArray[6]),float.Parse(sArray[7]));

			if (sArray [0] == "l") {
				leftHandExists = true;
				if (currentLeftHand == null) {
					currentLeftHand = Instantiate (leftHandPrefab, this.transform);
				}

				currentLeftHand.transform.GetChild(1).transform.position = position1;
				currentLeftHand.transform.GetChild(1).transform.rotation = rotation1;

			} else if (sArray [0] == "r") {
				rightHandExists = true;
				if (currentRightHand == null) {
					currentRightHand = Instantiate (rightHandPrefab, this.transform);
				}

				currentRightHand.transform.GetChild(1).transform.position = position1;
				currentRightHand.transform.GetChild(1).transform.rotation = rotation1;
			}
		}

		if (sArray.Length > 9) {
			sArray[9] = sArray [9].TrimStart ('(');
			sArray [11] = sArray [11].TrimEnd (')');
			sArray [12] = sArray [12].TrimStart ('(');
			sArray [15] = sArray [15].TrimEnd (')');


			position2 = new Vector3(float.Parse(sArray[9]),float.Parse(sArray[10]),float.Parse(sArray[11]));
			rotation2 = new Quaternion(float.Parse(sArray[12]),float.Parse(sArray[13]),float.Parse(sArray[14]),float.Parse(sArray[15]));


			if (sArray [8] == "l") {
				leftHandExists = true;
				if (currentLeftHand == null) {
					currentLeftHand = Instantiate (leftHandPrefab, this.transform);
				}

				currentLeftHand.transform.GetChild(1).transform.position = position2;
				currentLeftHand.transform.GetChild(1).transform.rotation = rotation2;

			} else if (sArray [8] == "r") {
				rightHandExists = true;
				if (currentRightHand == null) {
					currentRightHand = Instantiate (rightHandPrefab, this.transform);
				}

				currentRightHand.transform.GetChild(1).transform.position = position2;
				currentRightHand.transform.GetChild(1).transform.rotation = rotation2;
			}
		}

		if (!leftHandExists) {
			Destroy (currentLeftHand);
		}
		if (!rightHandExists) {
			Destroy (currentRightHand);
		}
	}

	void DestroyHands(){

		foreach (Transform child in gameObject.transform) {

			Destroy (child.gameObject);
		}
	}

	// receive thread
	void ReceiveData()
	{
		client = new UdpClient(port);
		IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

		while (true)
		{
			byte[] data = client.Receive(ref anyIP);

			string text = Encoding.UTF8.GetString(data);

			// split the items
			sArray = text.Split(',');

			if (sArray[0] != "nothing"){

				shouldUpdateHands =  true;

			} else {

				shouldDestroyHands = true;
			}

		}
	}
}
