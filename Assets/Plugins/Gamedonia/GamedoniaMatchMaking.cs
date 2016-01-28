using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;

namespace Gamedonia.Backend {
	public class GamedoniaMatchMaking  {

		public GamedoniaMatchMaking () {		
		}
		
		public static void CreateMatch(GDMatch match, Action<bool, GDMatch> callback = null) {

			string json = JsonMapper.ToJson(match);		
					
			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.post("/matchmaking/create",json,null, GamedoniaUsers.GetSessionToken(), null,
					delegate (bool success, object data) {
						GDMatch createdMatch = null;
						if (success) createdMatch =  DeserializeMatch((string) data);
						if (callback!=null) callback(success, createdMatch);				
					}
			 	 )
			);
		}


		public static void GetMatch(string matchId, Action<bool, GDMatch, List<GDMatchEvent>> callback = null) {

			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.get("/matchmaking/get/" + matchId,GamedoniaUsers.GetSessionToken(), 
			    	delegate (bool success, object data) {
						GDMatch match = null;
						List<GDMatchEvent> events = new List<GDMatchEvent>();
						if (success) {
							Dictionary<string,object> result = (Dictionary<string,object>)Json.Deserialize((string)data);						
							match = DeserializeMatch(result["match"] as Dictionary<string,object>);
							events = DeserializeMatchEvents(result["events"] as List<object>);
						}
						if (callback!=null) callback(success, match, events);				
					}	
				)
			);
		}

		public static void GetMatches(string state, int limit=0, string sort=null, int skip=0, Action<bool, List<GDMatch>,Dictionary<string,List<GDMatchEvent>>> callback = null) {

			string queryString = "state=" + state;
			if (limit>0) queryString += "&limit="+limit;
			if (sort!=null) queryString += "&sort=" + Uri.EscapeDataString(sort);
			if (skip>0) queryString += "&skip="+skip;

			string url = "/matchmaking/search?" + queryString;
			Debug.Log (url);
			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.get(url,GamedoniaUsers.GetSessionToken(), 
			    	delegate (bool success, object data) {
						List<GDMatch> matches = new List<GDMatch>();
						Dictionary<string,List<GDMatchEvent>> events = new Dictionary<string, List<GDMatchEvent>>();
						if(success) {
							Debug.Log ("GetMatches worked!");
							Dictionary<string,object> result = (Dictionary<string,object>)Json.Deserialize((string)data);
							matches = DeserializeMatches(result["matches"] as List<object>);
							events = DeserializeMatchEvents(result["events"] as Dictionary<string,object>);
						}
						if (callback!=null) callback(success, matches,events);				
					}	
				)
			);
		}

		public static void FindMatches(string query, int limit=0, Action<bool, List<GDMatch>> callback = null) {

			string url = "/matchmaking/find?query=" + query;
			if (limit>0) url += "&limit="+limit;

			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.get(url,GamedoniaUsers.GetSessionToken(), 
			    	delegate (bool success, object data) {
						if (callback!=null) {
							if (success) {
								if ((data==null) ||(data.Equals("[]")) )
									callback(success, new List<GDMatch>());
								else {
									List<GDMatch> matches = DeserializeMatches((string)data);
									callback(success,matches);
								}	
							}else {
								callback(success,null);
							}							
						}
					}	
				)
			);
		}

		public static void JoinMatch(string matchId, Action<bool, GDMatch> callback = null) {
			
			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.post("/matchmaking/join/" + matchId,"{}",null, GamedoniaUsers.GetSessionToken(), null,
			    	delegate (bool success, object data) {
						GDMatch joinedMatch = null;
						if (success) joinedMatch = DeserializeMatch((string) data);													
						if (callback!=null) callback(success, joinedMatch);				
					}
				)
			);

		}

		public static void LeaveMatch(string matchId, Action<bool, GDMatch> callback = null) {
			
			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.post("/matchmaking/join/" + matchId,"{}",null, GamedoniaUsers.GetSessionToken(), null,
			    	delegate (bool success, object data) {
						GDMatch leftMatch = null;
						if (success) leftMatch = DeserializeMatch((string) data);								
						if (callback!=null) callback(success, leftMatch);				
					}
				)
			);
			
		}

		public static void StartMatch(string matchId, Action<bool, GDMatch> callback = null) {
			
			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.post("/matchmaking/start/" + matchId,"{}",null, GamedoniaUsers.GetSessionToken(), null,
			    	delegate (bool success, object data) {
						GDMatch startedMatch = null;
						if (success) startedMatch = DeserializeMatch((string) data);							
						if (callback!=null) callback(success, startedMatch);				
					}
				)
			);
			
		}

		public static void FinishMatch(string matchId, string winnerUserId = "", Action<bool, GDMatch> callback = null) {

			IDictionary<string,object> inputData = new Dictionary<string, object> ();
			if ("".Equals(winnerUserId)) inputData.Add ("winnerId",winnerUserId);

			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.post("/matchmaking/finish/" + matchId,Json.Serialize(inputData),null, GamedoniaUsers.GetSessionToken(), null,
			    	delegate (bool success, object data) {
						GDMatch finishedMatch = null;
						if (success) finishedMatch = DeserializeMatch((string) data);					
						if (callback!=null) callback(success, finishedMatch);				
					}
				)
			);
			
		}

		/*
		 * TODO Easy method for matchmaking
		 */
		public static void QuickMatch(GDMatch match, string criteria, int requiredPlayers, int searchTime, Action<bool, GDMatch> callback= null) {

			GamedoniaMatchMaking.FindMatches (criteria, 0, 
			    delegate(bool success, List<GDMatch> foundMatches) {
					
					if (success) {
						if (foundMatches.Count > 0) {
							//Join Match
							GamedoniaMatchMaking.JoinMatch(foundMatches[0]._id,
						        delegate(bool joinSuccess, GDMatch joinedMatch) {
									
									if (success) {
										GamedoniaBackend.RunCoroutine(
											GamedoniaMatchMaking.IsMatchReady(joinedMatch._id,requiredPlayers, searchTime,
									            delegate(bool isReady) {
													
													if (isReady) {
														GamedoniaMatchMaking.StartMatch(joinedMatch._id,
											                delegate(bool startSucces, GDMatch startedMatch) {
																if(startSucces) {
																	callback(true,startedMatch);
																}else {
																	callback(false,null);
																}
															}
														);
													}else {
														callback(false,null);
													}
														
												}
											)	
										);
									}

								}
							);
						}else {

							GamedoniaMatchMaking.CreateMatch(match,delegate(bool successCreate, GDMatch createdMatch) {
								if (successCreate) {

									GamedoniaBackend.RunCoroutine(
										GamedoniaMatchMaking.IsMatchReady(createdMatch._id,requiredPlayers, searchTime,
									    	delegate(bool isReady) {
										
												if (isReady) {
													GamedoniaMatchMaking.StartMatch(createdMatch._id,
													    delegate(bool startSucces, GDMatch startedMatch) {
															if(startSucces) {
																callback(true,startedMatch);
															}else {
																callback(false,null);
															}
														}
													);
												}else {
													callback(false,null);
												}									
											}
										)	
									);

								}
							});

							
						}
					}

				}
			);
		}

		
		private static IEnumerator IsMatchReady(string matchId, int requiredPlayers, int waitTime, Action<bool> callback = null) {

			int time = 0;
			bool IsMatchReady = false;
			bool IsMatchReadyDone = false;
			bool IsGetMatchDone = false;
			GDMatch retrievedMatch = null;

			do {

				Debug.Log("Checking if Match [" + matchId + "] is ready");

				GamedoniaMatchMaking.GetMatch (matchId, 
				    delegate(bool success, GDMatch match, List<GDMatchEvent> matchEvents) {

						IsGetMatchDone = true;
						if (success) {
							retrievedMatch = match;
						}								
					}
				);


				while (!IsGetMatchDone) {
					yield return null;
				}

				//Check if match is ready
				if (retrievedMatch.users.Count >= requiredPlayers) {
					IsMatchReady = true;
					IsMatchReadyDone = true;
				}else {

					//Check if we are under total wait time
					if (time < waitTime) {
						yield return new WaitForSeconds(3);
						time += 3;
					}else {
						IsMatchReadyDone = true;
					}
				}


			}while(!IsMatchReadyDone);


			callback (IsMatchReady);	

		}


		private static GDMatch DeserializeMatch (Dictionary<string,object> matchMap) {

			GDMatch match = new GDMatch ();
			match._id = matchMap ["_id"] as string;
			match.state = matchMap ["state"] as string;
			match.properties = matchMap ["properties"] as Dictionary<string,object>;
			List<object> usersList = matchMap ["users"] as List<object>;
			
			
			foreach(object userObj in usersList) {
				Dictionary<string,object> userMap = userObj as Dictionary<string,object>;
				GDUserProfile user = new GDUserProfile();
				user._id = userMap["_id"] as string;
				user.profile = userMap["profile"] as Dictionary<string,object>;
				match.users.Add(user);
			}
			
			
			
			return match;

		}

		private static GDMatch DeserializeMatch (string data) {

			Dictionary<string,object> matchMap = Json.Deserialize((string)data) as Dictionary<string,object>;
			return DeserializeMatch (matchMap);
		}

		private static List<GDMatch> DeserializeMatches (List<object> matches) {

			List<GDMatch> matchesList = new List<GDMatch> ();

			foreach (object matchObj in matches) {
				Dictionary<string,object> matchMap = matchObj as Dictionary<string,object>;
				GDMatch match = new GDMatch ();
				match._id = matchMap ["_id"] as string;
				match.state = matchMap ["state"] as string;
				match.properties = matchMap ["properties"] as Dictionary<string,object>;
				List<object> usersList = matchMap ["users"] as List<object>;
				
				
				foreach(object userObj in usersList) {
					Dictionary<string,object> userMap = userObj as Dictionary<string,object>;
					GDUserProfile user = new GDUserProfile();
					user._id = userMap["_id"] as string;
					user.profile = userMap["profile"] as Dictionary<string,object>;
					match.users.Add(user);
				}
				
				matchesList.Add(match);
			}
			return matchesList;
		}

		private static List<GDMatch> DeserializeMatches (string data) {

			List<object> matches = Json.Deserialize ((string)data) as List<object>;
			return DeserializeMatches (matches);

		}

		private static Dictionary<string,List<GDMatchEvent>> DeserializeMatchEvents(Dictionary<string,object> eventsMap) {

			Dictionary<string, List<GDMatchEvent>> eventsPerMatch = new Dictionary<string, List<GDMatchEvent>> ();

			foreach(KeyValuePair<string, object> entry in eventsMap)
			{
				List<GDMatchEvent> matchEvents = DeserializeMatchEvents((List<object>)entry.Value);
				eventsPerMatch.Add(entry.Key,matchEvents);
			}

			return eventsPerMatch;
		}


		private static List<GDMatchEvent> DeserializeMatchEvents (List<object> eventsList ) {

			List<GDMatchEvent> events = new List<GDMatchEvent> ();

			foreach (object eventObj in eventsList) {

				Dictionary<string,object> eventMap = (Dictionary<string,object>)eventObj;

				GDMatchEvent theEvent = new GDMatchEvent();
				theEvent._id = Convert.ToString(eventMap["_id"]);
				theEvent.eventType = Convert.ToInt32(eventMap["eventType"]);
				theEvent.previousState = Convert.ToString(eventMap["previousState"]);
				theEvent.newState = Convert.ToString(eventMap["newState"]);
				theEvent.userId = Convert.ToString(eventMap["userId"]);

				events.Add(theEvent);
			}

			return events;

		}
	}



	public class GDMatchConstants {

		public const int TTL = 30;
		public const string CREATED = "CREATED";
		public const string STARTED = "STARTED";
		public const string FINISHED = "FINISHED";

	}

	public class GDMatch {



		public string _id;
		public string state = GDMatchConstants.CREATED;
		public Dictionary<string,object> properties = new Dictionary<string, object>();
		public List<GDUserProfile> users = new List<GDUserProfile>();
		public string eloField;
		public int timeToLive = GDMatchConstants.TTL;

		
		public GDMatch ()
		{

		}
	}


	public class GDMatcheEventConstants {

		public const int STATE_CHANGED = 1;
		public const int USER_JOINED = 2;
		public const int USER_LEFT = 3;

	}

	public class GDMatchEvent {

		public string _id;
		public int eventType;
		public string previousState;
		public string newState;
		public string userId;
		
	}
}
