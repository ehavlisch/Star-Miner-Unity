using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

using Dialogues;
using Ai;
using Util;
using Triggers;
using Races;

namespace Events {

	public class EventFactory {

        /*
            Event ideas:
            Warship fleet travelling, a bunch of ships flying in a pattern fly through the system (hostile?)
            Asteroid storm, fling asteroids at the player
            A probe sends out a pulse, could be an advertisement, low battery warning, service request, relayed message from a further probe
        */
		
		public static RandomEvent generateEvent() {
			switch(UnityEngine.Mathf.FloorToInt(UnityEngine.Random.Range(0, 2))) {
			case 2: {
				return strandedShipEvent();
			}
			case 1: {
				return abandonedShipEvent();
			}
			case 0: {
				return anomalyEvent();
			}
			default:
				Debug.Log("Default Random Event");
				//System.out.println("DEFAULT RANDOM EVENT");
				return null;
			}
		}
		
		private static RandomEvent strandedShipEvent() {
            Race race = RaceFactory.getRace();
            string id = race.generateName();
			
			string[] yesstrings = {"You: Sure, I can spare some fuel.", "You: Can't leave a fellow pilot stranded, of course.", "You: Take it and go, get away from those pirates.", "You: You owe me a drink.", "You: Sending some fuel your way. Watch that gauge a little closer next time."};
			string[] stillNostrings = {"You: I still cannot give you any fuel.", "You: I'm sorry, No.", "You: I was clear the first time.", "You: Nope.", "You: Answer is no. Good day sir."};
			string[] nostrings = { "You: Sorry friend, no can do.", "You: I'm sorry, I can't spare any.", "You: Nope.", "You: That's rough, nothing I can do.", "You: Bummer. Good luck."};
			
			string[] firstContactstrings = {"Hello? This is %ID%, we're stranded here out of fuel. Can you spare any? There's pirates on our heels!", "This is %ID%. Requesting fuel support urgently. Pirates in pursuit. Over.", "%ID% requests fuel aid. Can you spare any fuel for %ID%?", "Hey stranger, I could really use some fuel over here. Kinda stuck. Kinda been stuck a few days now. Can you spare anything for lil ol me?", "Hey, this is %ID%. We're out of fuel and can only travel on releasing air out of the airlock. Anyway you could give us some fuel and help us on our way again?", "Come in unidentified ship, this is %ID% broadcasting a distress message requesting fuel aid.", "This is %ID%. We are in distress, I repeat we are in distress. We are dead in the water here and desperately need fuel."};
			string[] beggingstrings = {"I'm begging you man! We've been stuck out here for days now. Nutrient paste will only last us another few days!", "You're signing our death certificate! No one else is going to find us before food runs out. I'm begging here!", "Please please please please... please. Please!", "What would I have to do to get some fuel? I'd do anything right about now. I mean it. Anything.", "Aww Geez. I'm on one knee here. I could really, really use the help.", "Please reconsider. Please?", "There's no way you're this heartless. What if no one else comes along? You're condemning me to death right now."};
			string[] warningShotstrings = {"There's a lot more where that came from. Help us or we'll unleash hell.", "That was deliberate. Don't make any rash decisions you may regret soon.", "Warning shot fired, help us or the next one won't be a warning anymore.", "Whoops, missfire. I swear. You ok? We still really need that fuel over here."};
			string[] takeFuelByForcestrings = {"Well if you won't help us, we'll take the fuel by force. Sorry it came to this.", "You've forced our hand then. Open Fire!", "Jackass.", "Shields up! Battlestations, we'll get our fuel one way or another!", "Prepare yourself. Your greed will cost you your life.", "There's nothing else we need to talk about then.", "Ok, don't take this the wrong way then."};
			string[] worthYourWhilestrings = {"Cmon man! We're completely stuck here! We can make it worth your while!", "We saved some of our most valuable cargo from the pirates and dumped the rest. We can share it with you for some fuel.", "We can pay you in some top notch cargo. We won't be able to sell any of it without any fuel.", "Please, we can pay! We have some cargo you might be interested in. It's not that cheap crap either."};
			string[] dealingWithRejectionstrings = {"God Damnit! Go fu...", "Christ... back to waiting for someone with a heart.", "Damnit.", "Thanks for nothing.", "Ok, I understand.", "I hope we don't meet in a dark alley sometime soon.", "May god have mercy on your pitiful soul.", "You are a wicked, cruel person. I hope you fall down a mineshaft and break your legs suffering for days before dying. Too far?" };
			string[] thanksHeresNothingstrings = {"Thank you so much, we'll be sure to pay it forward.", "Good luck out there, truly thank you so much.", "Sorry we dumped all of the cargo to run from the pirates, we have nothing to repay you with.", "Come by the bar some time, the %ID% owes you a drink."};
			string[] thanksHeresStuffstrings = {"We appreciate it immensely! We kept some of the most valuable cargo and dumped the rest. Take some and thank you.", "Here's what you're owed. Hopefully you're even. Good luck out there.", "Thanks much friend, as promised, here's the goods.", "Ok, we're clearing out. Grab this and then we suggest you take off too."};
			
			int fuelAmount = 5;
			
			string yesOptionLabel = "Yes (Send " + fuelAmount + " Fuel)";
			string stillNoOptionLabel = "Still No";
			string noOptionLabel = "No";
			
			List<AiBehavior> aiBehaviors = new List<AiBehavior>();
			aiBehaviors.Add(new AiBehavior(/*Move slowly, target planet*/));
			
			EventOption yesRewardOption = new EventOption(yesOptionLabel, Utils.getMember(yesstrings), Utils.getMember(thanksHeresStuffstrings), ActionResult.DROP_CARGO, ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT);
			yesRewardOption.setNumber(fuelAmount);
			yesRewardOption.setAiBehaviors(aiBehaviors);

			EventOption yesNoRewardOption = new EventOption(yesOptionLabel, Utils.getMember(yesstrings), Utils.getMember(thanksHeresNothingstrings), ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT);
			yesNoRewardOption.setNumber(fuelAmount);
			yesNoRewardOption.setAiBehaviors(aiBehaviors);
			
			List<EventOption> options = new List<EventOption>();
			options.Add(yesRewardOption);
			options.Add(new EventOption(stillNoOptionLabel, Utils.getMember(stillNostrings), Utils.getMember(dealingWithRejectionstrings), ActionResult.EXIT));
			
			Dialogue worthYourWhileDialogue = new Dialogue(Utils.getMember(worthYourWhilestrings), options);
			
			options = new List<EventOption>();
			options.Add(yesNoRewardOption);
			options.Add(new EventOption(stillNoOptionLabel, Utils.getMember(stillNostrings), Utils.getMember(dealingWithRejectionstrings), ActionResult.EXIT));
			
			Dialogue beggingDialogue = new Dialogue(Utils.getMember(beggingstrings), options);
			
			options = new List<EventOption>();
			
			if(Utils.nextBoolean()) {
				options.Add(yesRewardOption);
			} else {
				options.Add(yesNoRewardOption);
			}
			
			switch(Utils.randomInt(0, 3)) {
			case 0: {			
				EventOption warningShotOption = new EventOption(noOptionLabel, Utils.getMember(nostrings), worthYourWhileDialogue, Utils.getMember(warningShotstrings), ActionResult.UPDATE_AI_BEHAVIOR);
				aiBehaviors = new List<AiBehavior>();
				aiBehaviors.Add(new AiBehavior(/*Stationary, Fire warning shot*/));
				warningShotOption.setAiBehaviors(aiBehaviors);
				options.Add(warningShotOption);
				break;
			}
			case 1: {			
				options.Add(new EventOption(noOptionLabel, Utils.getMember(nostrings), Utils.getMember(takeFuelByForcestrings), ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT));
				break;
			}
			case 2: {
				options.Add(new EventOption(noOptionLabel, Utils.getMember(nostrings), worthYourWhileDialogue));
				break;
			}
			default: {
				options.Add(new EventOption(noOptionLabel, Utils.getMember(nostrings), beggingDialogue));
				break;
			}
			}
			Dialogue dialogue = new Dialogue(getRandomstring(firstContactstrings, id), options);
			
			DialogueTree dialogueTree = new DialogueTree(dialogue);
			
			return new RandomEvent("strandedShip", dialogueTree, id);	
		}
		
		private static RandomEvent abandonedShipEvent() {
            Race race = RaceFactory.getRace();
            string id = race.generateName();
			
			string unsureHelloOptionLabel = "Hello?";
			string repeatHelloOptionLabel = "Repeat: Hello?";
			
			string[] unsureHellostrings = {"You: Hello? Is there anything I can do to help?", "You: Hello? ... Hello?", "You: Hello? Are you ok?", "You: Hello? Anyone?", "You: I'm here to help! Who needs rescuing?", "You: Hello there... Hello?"};
			string[] talkingToMyselfstrings = {"You: Well now I'm just talking to myself.", "You: (Singing) Hello? Is there anybody in there? Is there any-body hoooome?", "You: ?", "You: (Counter the silence with silence)", "You: (Attack the silence mentally)", "You: Hellooooo?", "You: Well, I'll just go then...", "You: I often repeat repeat myself. I often repeat repeat.", "You: In fact I think it's neat it's neat. to to to to repeat repeat", "You: I wonder if there's an end to the loop", "You: (Knows this is pointless now)", "You: (How deep does the rabbit hole go?)", "You: (Is this a new message?)"};
			string[] lurestrings = {"%ID% broadcasting automated distress message. Unconcious pilot detected. Requesting aid.", "(The coms are open, but you only hear silence)", "(The coms are open, but you only hear a faint beeping and a faint tapping)", "Automated Distres Signal Activated %ID%: General Distress Code: 0", "(The coms are open and you hear a soft cloth shift before silence)", "Automated Message: Derelict Ship %ID% - Board at your own risk."};
			string[] noFurtherResponsestrings = {"(Silence)", "...", "(Loud silence)", "(Medium silence)", "(Deafening silence)", "(Quiet)", "(No response)", "(Coms stay open)", "(No one is listening)", "(The void listens)", "(Your message drifts into the void unanswered)", "(The silence wins)"};		
			
			string[] boobyTrapstrings = {"(Something is wrong)", "(You hear a faint click)", "(You hear a beeping suddenly stop)", "(A laugh destroys the silence)", "(The ship glows brighter)", "(Hair raises on the back of your neck)", "(It's quiet, too quiet)"};
			string[] ambushstrings = {"Target in range! Get 'im!", "Suprise Mother Trucker", "Hope you got some booty in that hold for me!", "Break cloak! Aim for the engines!", "(Maniacal Laughing)", "Here's Johnny!"};
			
			Dialogue noResponseLoopDialogue = new Dialogue(Utils.getMember(noFurtherResponsestrings), null);
			noResponseLoopDialogue.setupLoop(repeatHelloOptionLabel, talkingToMyselfstrings, noFurtherResponsestrings);
			
			List<EventOption> lureOptions = new List<EventOption>();		
			
			switch(Utils.randomInt(0, 4)) {
			case 0: {
				lureOptions.Add(new EventOption(unsureHelloOptionLabel, Utils.getMember(unsureHellostrings), noResponseLoopDialogue));
				break;
			}
			case 1: {
				EventOption boobyTrapEventOption = new EventOption(unsureHelloOptionLabel, Utils.getMember(unsureHellostrings), noResponseLoopDialogue, Utils.getMember(noFurtherResponsestrings), ActionResult.ADD_TRIGGER);
				Trigger boobyTrapTrigger = new Trigger("Booby Trap Proximity Trigger", null /*Trigger Event Type Proximity*/);
				boobyTrapTrigger.setComsMessage(Utils.getMember(boobyTrapstrings));
				boobyTrapEventOption.addTrigger(boobyTrapTrigger);
				lureOptions.Add(boobyTrapEventOption);
				break;
			}
			case 2: {
				List<AiBehavior> aiBehaviors = new List<AiBehavior>();
				aiBehaviors.Add(new AiBehavior(/*Wander, Normal Speed*/));
				
				EventOption shipJustGoesEventOption = new EventOption(unsureHelloOptionLabel, Utils.getMember(unsureHellostrings), "", ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT);
				shipJustGoesEventOption.setAiBehaviors(aiBehaviors);
				lureOptions.Add(shipJustGoesEventOption);
				break;
			}
			default: 
				EventOption ambushEventOption = new EventOption(unsureHelloOptionLabel, Utils.getMember(unsureHellostrings), noResponseLoopDialogue, Utils.getMember(noFurtherResponsestrings), ActionResult.ADD_TRIGGER);
				Trigger ambushTrigger = new Trigger("Ambush Proximity Trigger", null /*Trigger Event Type Proximity*/);
				ambushTrigger.setWorldObjects(null /*List of ambush ships*/);
				ambushTrigger.setComsMessage(Utils.getMember(ambushstrings));
				ambushEventOption.addTrigger(ambushTrigger);
				
				lureOptions.Add(ambushEventOption);
				break;
			}
			
			Dialogue lureDialogue = new Dialogue(getRandomstring(lurestrings, id), lureOptions);
			
			DialogueTree dialogueTree = new DialogueTree(lureDialogue);
			
			return new RandomEvent("ambush", dialogueTree, id);
		}
		
		private static RandomEvent anomalyEvent() {
			
			string scanAnomalyOptionLabel = "Scan";
			
			string anomalystring = "An odd anomaly has appeared near you.";
			string anomalyActionstring = "The anomaly pulses and disappears.";
			
			List<EventOption> anomalyOptions = new List<EventOption>();
			anomalyOptions.Add(new EventOption(scanAnomalyOptionLabel, anomalyActionstring, "", ActionResult.EXIT));
			
			
			Dialogue anomalyDialogue = new Dialogue(anomalystring, anomalyOptions);
			
			
			DialogueTree dialogueTree = new DialogueTree(anomalyDialogue);
			
			RandomEvent evt = new RandomEvent("Anomaly", dialogueTree,"???");
			evt.setComs(false);
			return evt;
		}
		
		public static string getRandomstring(string[] strings, string id) {
            string s = Utils.getMember(strings);
            s = s.Replace("%ID%", id);
			return s;
		}

        
        public static string sortIds(string[] ids) {
			SortedList<string, string> sorted = new SortedList<string, string>();
			for(int i = 0; i < ids.Length; i++) {
				try {
					sorted.Add(ids[i], ids[i]);
				} catch(Exception e) {
					Debug.Log ("Duplicate: " + ids[i] + "(" + e.Message + ")");
				}
			}
			StringBuilder sb = new StringBuilder();
			sb.Append("{\n");

            //sorted.Values[0].First().ToString().ToUpper() + input.Substring(1);

            char c = sorted.Values[0].ToCharArray()[0];
			ICollection<string> keys = sorted.Keys;
			foreach(string id in keys) {
                string upper = id.Substring(0, 1).ToUpper() + id.Substring(1);
				if(c == upper.ToCharArray()[0]) {
					sb.Append("\"").Append(upper).Append("\", ");
				} else {
					c = upper.ToCharArray()[0];
					sb.Append("\n\"").Append(upper).Append("\", ");
				}
			}
			sb.Remove (sb.Length - 2, 2);
			
			sb.Append("\n};");

            return sb.ToString();
            /*
            // Does some stuff to writ to unity logs...
            List<string> idstrings = new List<string>();
            string allIds = sb.ToString();
            if (allIds.Length > 16000) {
                int offset = 0;
                while(offset < allIds.Length) {
                    if (allIds.Length < offset + 16000) {
                        idstrings.Add(allIds.Substring(offset));
                    } else {
                        idstrings.Add(allIds.Substring(offset, 16000));
                    }
                    offset += 16000;
                }
            } else {
                idstrings.Add(allIds);
            }
            
            return idstrings;
            */
		}
	}
}