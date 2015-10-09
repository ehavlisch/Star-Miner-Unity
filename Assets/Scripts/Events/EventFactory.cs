using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using DialogueNS;
using Ai;
using Util;
using Triggers;

namespace Events {

	public class EventFactory {
		
		public static Event generateEvent() {
			//		int eventsSize = 1;
			
			//		new Random().nextInt(eventsSize))
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
		
		private static Event strandedShipEvent() {
			
			string id = generateId();
			
			string[] yesStrings = {"You: Sure, I can spare some fuel.", "You: Can't leave a fellow pilot stranded, of course.", "You: Take it and go, get away from those pirates.", "You: You owe me a drink.", "You: Sending some fuel your way. Watch that gauge a little closer next time."};
			string[] stillNoStrings = {"You: I still cannot give you any fuel.", "You: I'm sorry, No.", "You: I was clear the first time.", "You: Nope.", "You: Answer is no. Good day sir."};
			string[] noStrings = { "You: Sorry friend, no can do.", "You: I'm sorry, I can't spare any.", "You: Nope.", "You: That's rough, nothing I can do.", "You: Bummer. Good luck."};
			
			string[] firstContactStrings = {"Hello? This is %ID%, we're stranded here out of fuel. Can you spare any? There's pirates on our heels!", "This is %ID%. Requesting fuel support urgently. Pirates in pursuit. Over.", "%ID% requests fuel aid. Can you spare any fuel for %ID%?", "Hey stranger, I could really use some fuel over here. Kinda stuck. Kinda been stuck a few days now. Can you spare anything for lil ol me?", "Hey, this is %ID%. We're out of fuel and can only travel on releasing air out of the airlock. Anyway you could give us some fuel and help us on our way again?", "Come in unidentified ship, this is %ID% broadcasting a distress message requesting fuel aid.", "This is %ID%. We are in distress, I repeat we are in distress. We are dead in the water here and desperately need fuel."};
			string[] beggingStrings = {"I'm begging you man! We've been stuck out here for days now. Nutrient paste will only last us another few days!", "You're signing our death certificate! No one else is going to find us before food runs out. I'm begging here!", "Please please please please... please. Please!", "What would I have to do to get some fuel? I'd do anything right about now. I mean it. Anything.", "Aww Geez. I'm on one knee here. I could really, really use the help.", "Please reconsider. Please?", "There's no way you're this heartless. What if no one else comes along? You're condemning me to death right now."};
			string[] warningShotStrings = {"There's a lot more where that came from. Help us or we'll unleash hell.", "That was deliberate. Don't make any rash decisions you may regret soon.", "Warning shot fired, help us or the next one won't be a warning anymore.", "Whoops, missfire. I swear. You ok? We still really need that fuel over here."};
			string[] takeFuelByForceStrings = {"Well if you won't help us, we'll take the fuel by force. Sorry it came to this.", "You've forced our hand then. Open Fire!", "Jackass.", "Shields up! Battlestations, we'll get our fuel one way or another!", "Prepare yourself. Your greed will cost you your life.", "There's nothing else we need to talk about then.", "Ok, don't take this the wrong way then."};
			string[] worthYourWhileStrings = {"Cmon man! We're completely stuck here! We can make it worth your while!", "We saved some of our most valuable cargo from the pirates and dumped the rest. We can share it with you for some fuel.", "We can pay you in some top notch cargo. We won't be able to sell any of it without any fuel.", "Please, we can pay! We have some cargo you might be interested in. It's not that cheap crap either."};
			string[] dealingWithRejectionStrings = {"God Damnit! Go fu...", "Christ... back to waiting for someone with a heart.", "Damnit.", "Thanks for nothing.", "Ok, I understand.", "I hope we don't meet in a dark alley sometime soon.", "May god have mercy on your pitiful soul.", "You are a wicked, cruel person. I hope you fall down a mineshaft and break your legs suffering for days before dying. Too far?" };
			string[] thanksHeresNothingStrings = {"Thank you so much, we'll be sure to pay it forward.", "Good luck out there, truly thank you so much.", "Sorry we dumped all of the cargo to run from the pirates, we have nothing to repay you with.", "Come by the bar some time, the %ID% owes you a drink."};
			string[] thanksHeresStuffStrings = {"We appreciate it immensely! We kept some of the most valuable cargo and dumped the rest. Take some and thank you.", "Here's what you're owed. Hopefully you're even. Good luck out there.", "Thanks much friend, as promised, here's the goods.", "Ok, we're clearing out. Grab this and then we suggest you take off too."};
			
			int fuelAmount = 5;
			
			string yesOptionLabel = "Yes (Send " + fuelAmount + " Fuel)";
			string stillNoOptionLabel = "Still No";
			string noOptionLabel = "No";
			
			List<AiBehavior> aiBehaviors = new List<AiBehavior>();
			aiBehaviors.Add(new AiBehavior(/*Move slowly, target planet*/));
			
			EventOption yesRewardOption = new EventOption(yesOptionLabel, getRandomString(yesStrings), getRandomString(thanksHeresStuffStrings), ActionResult.DROP_CARGO, ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT);
			yesRewardOption.setNumber(fuelAmount);
			yesRewardOption.setAiBehaviors(aiBehaviors);

			EventOption yesNoRewardOption = new EventOption(yesOptionLabel, getRandomString(yesStrings), getRandomString(thanksHeresNothingStrings), ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT);
			yesNoRewardOption.setNumber(fuelAmount);
			yesNoRewardOption.setAiBehaviors(aiBehaviors);
			
			List<EventOption> options = new List<EventOption>();
			options.Add(yesRewardOption);
			options.Add(new EventOption(stillNoOptionLabel, getRandomString(stillNoStrings), getRandomString(dealingWithRejectionStrings), ActionResult.EXIT));
			
			Dialogue worthYourWhileDialogue = new Dialogue(getRandomString(worthYourWhileStrings), options);
			
			options = new List<EventOption>();
			options.Add(yesNoRewardOption);
			options.Add(new EventOption(stillNoOptionLabel, getRandomString(stillNoStrings), getRandomString(dealingWithRejectionStrings), ActionResult.EXIT));
			
			Dialogue beggingDialogue = new Dialogue(getRandomString(beggingStrings), options);
			
			options = new List<EventOption>();
			
			if(Utils.nextBoolean()) {
				options.Add(yesRewardOption);
			} else {
				options.Add(yesNoRewardOption);
			}
			
			switch(Utils.randomInt(0, 3)) {
			case 0: {			
				EventOption warningShotOption = new EventOption(noOptionLabel, getRandomString(noStrings), worthYourWhileDialogue, getRandomString(warningShotStrings), ActionResult.UPDATE_AI_BEHAVIOR);
				aiBehaviors = new List<AiBehavior>();
				aiBehaviors.Add(new AiBehavior(/*Stationary, Fire warning shot*/));
				warningShotOption.setAiBehaviors(aiBehaviors);
				options.Add(warningShotOption);
				break;
			}
			case 1: {			
				options.Add(new EventOption(noOptionLabel, getRandomString(noStrings), getRandomString(takeFuelByForceStrings), ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT));
				break;
			}
			case 2: {
				options.Add(new EventOption(noOptionLabel, getRandomString(noStrings), worthYourWhileDialogue));
				break;
			}
			default: {
				options.Add(new EventOption(noOptionLabel, getRandomString(noStrings), beggingDialogue));
				break;
			}
			}
			Dialogue dialogue = new Dialogue(getRandomString(firstContactStrings, id), options);
			
			DialogueTree dialogueTree = new DialogueTree(dialogue);
			
			return new Event("strandedShip", dialogueTree, id);	
		}
		
		private static Event abandonedShipEvent() {
			string id = generateId();
			
			string unsureHelloOptionLabel = "Hello?";
			string repeatHelloOptionLabel = "Repeat: Hello?";
			
			string[] unsureHelloStrings = {"You: Hello? Is there anything I can do to help?", "You: Hello? ... Hello?", "You: Hello? Are you ok?", "You: Hello? Anyone?", "You: I'm here to help! Who needs rescuing?", "You: Hello there... Hello?"};
			string[] talkingToMyselfStrings = {"You: Well now I'm just talking to myself.", "You: (Singing) Hello? Is there anybody in there? Is there any-body hoooome?", "You: ?", "You: (Counter the silence with silence)", "You: (Attack the silence mentally)", "You: Hellooooo?", "You: Well, I'll just go then...", "You: I often repeat repeat myself. I often repeat repeat.", "You: In fact I think it's neat it's neat. to to to to repeat repeat", "You: I wonder if there's an end to the loop", "You: (Knows this is pointless now)", "You: (How deep does the rabbit hole go?)", "You: (Is this a new message?)"};
			string[] lureStrings = {"%ID% broadcasting automated distress message. Unconcious pilot detected. Requesting aid.", "(The coms are open, but you only hear silence)", "(The coms are open, but you only hear a faint beeping and a faint tapping)", "Automated Distres Signal Activated %ID%: General Distress Code: 0", "(The coms are open and you hear a soft cloth shift before silence)", "Automated Message: Derelict Ship %ID% - Board at your own risk."};
			string[] noFurtherResponseStrings = {"(Silence)", "...", "(Loud silence)", "(Medium silence)", "(Deafening silence)", "(Quiet)", "(No response)", "(Coms stay open)", "(No one is listening)", "(The void listens)", "(Your message drifts into the void unanswered)", "(The silence wins)"};		
			
			string[] boobyTrapStrings = {"(Something is wrong)", "(You hear a faint click)", "(You hear a beeping suddenly stop)", "(A laugh destroys the silence)", "(The ship glows brighter)", "(Hair raises on the back of your neck)", "(It's quiet, too quiet)"};
			string[] ambushStrings = {"Target in range! Get 'im!", "Suprise Mother Trucker", "Hope you got some booty in that hold for me!", "Break cloak! Aim for the engines!", "(Maniacal Laughing)", "Here's Johnny!"};
			
			Dialogue noResponseLoopDialogue = new Dialogue(getRandomString(noFurtherResponseStrings), null);
			noResponseLoopDialogue.setupLoop(repeatHelloOptionLabel, talkingToMyselfStrings, noFurtherResponseStrings);
			
			List<EventOption> lureOptions = new List<EventOption>();		
			
			switch(Utils.randomInt(0, 4)) {
			case 0: {
				lureOptions.Add(new EventOption(unsureHelloOptionLabel, getRandomString(unsureHelloStrings), noResponseLoopDialogue));
				break;
			}
			case 1: {
				EventOption boobyTrapEventOption = new EventOption(unsureHelloOptionLabel, getRandomString(unsureHelloStrings), noResponseLoopDialogue, getRandomString(noFurtherResponseStrings), ActionResult.ADD_TRIGGER);
				Trigger boobyTrapTrigger = new Trigger("Booby Trap Proximity Trigger", null /*Trigger Event Type Proximity*/);
				boobyTrapTrigger.setComsMessage(getRandomString(boobyTrapStrings));
				boobyTrapEventOption.addTrigger(boobyTrapTrigger);
				lureOptions.Add(boobyTrapEventOption);
				break;
			}
			case 2: {
				List<AiBehavior> aiBehaviors = new List<AiBehavior>();
				aiBehaviors.Add(new AiBehavior(/*Wander, Normal Speed*/));
				
				EventOption shipJustGoesEventOption = new EventOption(unsureHelloOptionLabel, getRandomString(unsureHelloStrings), "", ActionResult.UPDATE_AI_BEHAVIOR, ActionResult.EXIT);
				shipJustGoesEventOption.setAiBehaviors(aiBehaviors);
				lureOptions.Add(shipJustGoesEventOption);
				break;
			}
			default: 
				EventOption ambushEventOption = new EventOption(unsureHelloOptionLabel, getRandomString(unsureHelloStrings), noResponseLoopDialogue, getRandomString(noFurtherResponseStrings), ActionResult.ADD_TRIGGER);
				Trigger ambushTrigger = new Trigger("Ambush Proximity Trigger", null /*Trigger Event Type Proximity*/);
				ambushTrigger.setWorldObjects(null /*List of ambush ships*/);
				ambushTrigger.setComsMessage(getRandomString(ambushStrings));
				ambushEventOption.addTrigger(ambushTrigger);
				
				lureOptions.Add(ambushEventOption);
				break;
			}
			
			Dialogue lureDialogue = new Dialogue(getRandomString(lureStrings, id), lureOptions);
			
			DialogueTree dialogueTree = new DialogueTree(lureDialogue);
			
			return new Event("ambush", dialogueTree, id);
		}
		
		private static Event anomalyEvent() {
			
			string scanAnomalyOptionLabel = "Scan";
			
			string anomalyString = "An odd anomaly has appeared near you.";
			string anomalyActionString = "The anomaly pulses and disappears.";
			
			List<EventOption> anomalyOptions = new List<EventOption>();
			anomalyOptions.Add(new EventOption(scanAnomalyOptionLabel, anomalyActionString, "", ActionResult.EXIT));
			
			
			Dialogue anomalyDialogue = new Dialogue(anomalyString, anomalyOptions);
			
			
			DialogueTree dialogueTree = new DialogueTree(anomalyDialogue);
			
			Event evt = new Event("Anomaly", dialogueTree,"???");
			evt.setComs(false);
			return evt;
		}
		
		public static string getRandomString(String[] strings) {
			string s = strings[Utils.randomInt(strings.Length)];
			return s;
		}
		
		public static string getRandomString(String[] strings, string id) {
			string s = strings[Utils.randomInt(strings.Length)];
			s = s.Replace("%ID%", id);
			return s;
		}
		
		public static string generateId() {
			string id = generateRandomPrimaryId();
			switch(Utils.randomInt(6)) {
			case 0: {
				id = id + generateRandomIdentifier(false) + generateRandomLinker() + generateRandomIdentifier(true);
				break;
			}
			case 1: {
				id = id + " " + generateRandomModifier() + generateRandomModifier();
				break;
			}
			case 2: {
				id = id + generateRandomIdentifier(false) + generateRandomLinker() + generateRandomModifier();
				break;
			}
			case 3: {
				id = id + "-" + generateRandomPrimaryId() + " " + generateRandomModifier();
				break;
			}
			case 4: {
				id = id + generateRandomLinker() + generateRandomPrimaryId() + " " + generateRandomFusionWord();
				break;
			}
			case 5: {
				id = id + " " + generateRandomModifier() + generateRandomLinker() + generateRandomModifier();
				break;
			}
			default: {
				id = "Default name";
				break;
			}
			}
			
			return id;
		}
		
		public static string generateRandomIdentifier(bool removeSpace) {
			string id = identifiers[Utils.randomInt(identifiers.Length)];
			
			if(removeSpace) {
				id = id.Substring(1, id.Length);
			}
			
			return id;
		}
		
		public static string generateRandomPrimaryId() {
			return ids[Utils.randomInt(ids.Length)];
		}
		
		public static string generateRandomModifier() {
			
			int length = Utils.randomInt(2) + 1;
			
			StringBuilder sb = new StringBuilder();
			
			for(int i = 0; i <= length; i++) {
				sb.Append(chars[(Utils.randomInt(chars.Length))]);
			}
			
			return sb.ToString();
		}
		
		private static char generateRandomLinker() {
			return linkers[Utils.randomInt(linkers.Length)];
		}
		
		private static string generateRandomFusionWord() {
			return fusionWords[Utils.randomInt(fusionWords.Length)];
		}
		
		private static string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ1234567890";
		
		private static string linkers = "-.:/\\| =+";
		
		private static string[] fusionWords = {
			"Fusion", "Hybrid", "Morph", "Combo", "Combination", "Mesh", "Fused", "Merged"
		};
		
		private static string[] ids = {
			"A-Wing", "Achilles", "Acidalia", "Adrastea", "Aegaeon", "Aegir", "Aitheria", "Aiti", "Aitimaa", "Aitne", "Aizea", "Ajax", "Akmar", "Alawi", "Albiorix", "Alhena", "Allita", "Almika", "Alzubra", "Amalthea", "Amalur", "Amalurra", "Ananke", "Andromeda", "Angel", "Ant", "Anthe", "Anubis", "Anubite", "Aoede", "Apache", "Aphrodite", "Apolline", "Apollo", "Apollonia", "Apostate", "Aqmar", "Arche", "Arcus", "Argon", "Argus", "Ariel", "Ars", "Artemis", "Arwing", "Asianne", "Asta", "Asteria", "Astra", "Astrea", "Astrolab", "Astrolux", "Atalanta", "Athena", "Atlas", "Auriok", "Aurum", "Automaton", "Autonoe", "Avenger", "Azban", "Azeroth", "Azure", 
			"Baast", "Bab-El-Sama", "Badra", "Badriyyah", "Baldr", "Baldur", "Ballista", "Banshee", "Bast", "Bear", "Beaver", "Bebhionn", "Behemoth", "Belinda", "Bellerphon", "Bergelmir", "Bestla", "Bianca", "Bireme", "Bishop", "Blackhand", "Blackwatch", "Bladeburn", "Blood Hunter", "Boar", "Boeing", "Bolas", "Boris", "Burnside", 
			"Caelestra", "Caeli", "Caelia", "Caesar", "Caladria", "Caliban", "Callirrhoe", "Callisto", "Calva", "Calypso", "Camel", "Cancer", "Capricorn", "Carbon", "Carcinos", "Carme", "Carnivora", "Carpo", "Carrier", "Celaeno", "Celesse", "Celesta", "Celestia", "Celestiel", "Celestina", "Celia", "Celine", "Centaur", "Century", "Ceunturion", "Chaldene", "Chamanja", "Chandra", "Chandrani", "Chandrika", "Chaplain", "Charon", "Chaska", "Chava", "Cheiroballista", "Cherika", "Chimera", "Chipara", "Cira", "Claimh Solas", "Cobalt", "Cochava", "Colossus", "Contarius", "Controller", "Copper", "Cordelia", "Cow", "Cressida", "Cruiser", "Crystal", "Cupid", "Cyclops", "Cyllene", 
			"Daichi", "Damia", "Dangeresque", "Daphnis", "Deimos", "Dellingr", "Desdemona", "Despina", "Destiny", "Developer", "Dia", "Diamond", "Dinger", "Dingo", "Dione", "Dionysus", "Donoma", "Doombringer", "Drakkar", "Dremora", "Dunia", "Duniya", "Dunya", "Dysnomia", 
			"Eagle", "Echo", "Einherjar", "Elara", "Eleana", "Elektra", "Eliana", "Elianna", "Elianne", "Elver", "Emperor", "Empire", "Enceladus", "Epimetheus", "Erinome", "Erriapus", "Essie", "Estee", "Estella", "Estelle", "Ester", "Esther", "Estrela", "Eszter", "Eszti", "Euanthe", "Eukelade", "Euporie", "Europa", "Eurydome", "Eustella", "Exodus", 
			"Facade", "Falak", "Falcon", "Falkreath", "Farbauti", "Fenrir", "Ferdinand", "Fimbulwinter", "Firdaus", "Firdaws", "Firdous", "Fochik", "Fornjot", "Forseti", "Francisco", "Freya", "Freyja", "Freyr", "Frost", 
			"Galatea", "Ganymede", "Gastraphetes", "Gemini", "Giant", "Gimle", "Goblin", "Goldenrod", "Goliath", "Gomorrah", "Gravekeeper", "Greip", "Gunner", 
			"Hades", "Hakidonmuya", "Hala", "Halimede", "Harpalyke", "Hathor", "Hati", "Hawk", "Hawkeye", "Hegemone", "Heimdall", "Heimdallr", "Heka Gigantes", "Hekate", "Hel", "Helene", "Helepolis", "Helia", "Helike", "Helios", "Hellbringer", "Hephaestus", "Hera", "Heracles", "Hercules", "Hermes", "Hermippe", "Herse", "Hersir", "Hester", "Hetairoi", "Hi'laka", "Hilal", "Himalia", "Hina", "Hippikon", "Hippocampus", "Hippolyta", "Honua", "Hoor", "Hoplite", "Hoshi", "Hoshiko", "Hoshiyo", "Hrunting", "Hullanta", "Huskarl", "Hydra", "Hypaspist", "Hyperion", "Hyrrokkin", "Höðr", "Hœnir", 
			"Iamar", "Ijiraq", "Indigo", "Indu", "Indukala", "Indulala", "Induma", "Industrial Freighter", "Infantry", "Intina", "Io", "Ion", "Iris", "Iron", "Isonoe", "Ixkin", "Izar", "Izarra", 
			"Jaguar", "Jango", "Janissary", "Janna", "Janus", "Jarl", "Jarnsaxa", "Jason", "Jata", "Jay", "Jellyfish", "Jelonik", "Jericho", "Jinan", "Jormund", "Juggernaut", "Juliet", "Jupiter", "Jyotsna", 
			"Kalani", "Kale", "Kallichore", "Kalyke", "Kamar", "Kamra", "Kane", "Kari", "Katapeltes", "Kawkab", "Kawthar", "Kealani", "Keeper", "Kerberos", "Khepri", "Kiania", "Kianira", "Kiche", "King", "Kingfisher", "Kingpin", "Kirov", "Kitten", "Kiviuq", "Knight", "Kong", "Kore", "Kraken", 
			"Lamb", "Lampades", "Lani", "Lanika", "Laomedeia", "Lapetus", "Larissa", "Lazarus", "Leda", "Legate", "Legion", "Legionaire", "Leilani", "Leolani", "Leto", "Levana", "Leviathan", "Lian", "Liane", "Lianna", "Liger Zero", "Lion", "Locaste", "Loge", "Loki", "Longboat", "Loni", "Lordchefmage", "Luan", "Luna", "Lunasa", "Lunetta", "Lurra", "Lysithea", 
			"Mab", "Magena", "Magmus", "Magnus", "Maia", "Maiden of the Mist", "Maleda", "Mammoth", "Man O' War", "Man of War", "Manticore", "Mapiya", "Marauder", "Maret", "Margaret", "Marinda", "Mars", "MedVac", "Medini", "Medusa", "Megaclite", "Meili", "Mercury", "Methone", "Metis", "Miakoda", "Migina", "Mikazuki", "Millenium", "Mimas", "Mimiteh", "Minion", "Minotaur", "Miranda", "Misae", "Mneme", "Mobius", "Mona", "Moon", "Moose", "Morndas", "Mountain", "Mourne", "Mummy", "Mundilfari", "Murmillo", "Myrmidon", "Mythril", 
			"Naiad", "Najm", "Najma", "Namaka", "Namid", "Narvi", "Nautilus", "Nemean", "Nephthys", "Neptune", "Nereid", "Neso", "Nidhogg", "Niji", "Nijina", "Nix", "Njord", "Njörðr", "Nyika", "Nymaane", "Nyota", 
			"Oberon", "Oceanus", "Ocelot", "Odin", "Odysseus", "Onile", "Ooljee", "Oota Dabun", "Ophelia", "Oracle", "Oriana", "Oriole", "Oro", "Orthosie", "Osiris", "Osprey", "Osumare", "Overlord", "Ox", 
			"Paaliaq", "Pachama", "Pachamama", "Paiva", "Paivatar", "Paladin", "Pallene", "Pamuya", "Pan", "Pandora", "Panther", "Pasiphae", "Pasture", "Pawn", "Peace", "Pegasus", "Peltast", "Perdita", "Perseus", "Petrobolos", "Petsuchos", "Phobos", "Phoebe", "Phoenix", "Pig", "Pleione", "Pluto", "Polani", "Polydeuces", "Polyphemus", "Portia", "Poseidon", "Prasithee", "Praxidike", "Priest", "Prodromos", "Promethean", "Prometheus", "Prospero", "Proteus", "Psamathe", "Ptah", "Puck", "Puma", "Purnima", 
			"Qamar", "Qamra", "Qoylurani", "Quark", "Queen", 
			"Ragnarok", "Rampage", "Raven", "Razorback", "Realta", "Renegade", "Restless", "Rhea", "Rindir", "Roach", "Roc", "Rook", "Rosalind", "Rubashov", "Rune", "Runic", "Rust", 
			"Sama", "Sanuye", "Sao", "Sarraqa", "Saturn", "Satyr", "Saxnot", "Scarab", "Scorpion", "Scylla", "Selena", "Selenia", "Serpent", "Setebos", "Shade", "Shanira", "Sheep", "Sheiramoth", "Shoteka", "Siarnaq", "Sidra", "Sif", "Silencer", "Sinope", "Sitara", "Skaoi", "Skathi", "Skoll", "Skyhawk", "Skyhold", "Skyrim", "Sol", "Sola", "Soldier", "Solveig", "Souhaila", "Sphinx", "Sponde", "Sputnik", "Stanislava", "Starfish", "Starlis", "Starr", "Steel", "Stella", "Stelle", "Stephano", "Strider", "Stymphalian", "Styx", "Suha", "Suhaila", "Sunn", "Sunniva", "Suraya", "Surtur", "Suttungr", "Sycorax", "Synne", "Synnove", 
			"Tadewi", "Taigi", "Taima", "Taini", "Tal", "Talia", "Taliya", "Talora", "Talori", "Talya", "Tarika", "Tarqeq", "Tartarian", "Tartarus", "Tarvos", "Taurina", "Taygete", "Tefia", "Telesto", "Tellus", "Tenshi", "Terrene", "Terricia", "Teruko", "Tethys", "Thalassa", "The Dev Made Me Do It", "Thebe", "Theia", "Thelxinoe", "Themisto", "Theseus", "Thor", "Thrymr", "Thuraya", "Thwayya", "Thyone", "Tin", "Tirdas", "Titan", "Titania", "Titanium", "Torturer", "Trident", "Trinculo", "Triton", "Troll", "Tukiko", "Tukiyo", "Tundra", "Turma", "Tuwa", "Tyr", 
			"Ulfsark", "Ullr", "Umbriel", "Undead", "Urania", "Uruk-hai", 
			"Vader", "Vali", "Valkyrie", "Vanessa", "Var", "Vega", "Vengeance", "Venus", "Vermouth", "Vespera", "Vesperia", "Vespira", "Vili", "Villager", "Vindicator", "Vioarr", "Virulent", "Virus", "Vor", 
			"Wadjet", "Wallaby", "Walrus", "War Turtle", "Warbringer", "Warface", "Warlike", "Warner", "Warren", "Warrior", "Water", "Weilani", "Wendel", "Wendigo", "Wicapiwakan", "Wilhelm", "Woodchuck", "Wraith", "Wrath", "Wumbo", 
			"X-Wing", "Xavier", "Xylo", 
			"Y-Wing", "Yager", "Yaxeka", "Yaxkin", "Ymir", "Yuri", 
			"Zero", "Zeus", "Ziazan", "Zisa", "Zonje", "Zonnetje", "Zuvan"
		};
		
		private static string[] identifiers = {
			" 0", " 00", " 000", 
			" 1", " 10", " 111", 
			" 2", " 222", 
			" 3", " 333", " 343", 
			" 4", " 404", " 444", 
			" 5", " 555", 
			" 66", " 666", " 69", 
			" 7", " 77", " 777", 
			" 888", 
			" 999", 
			" A", " AB", " AC", " AD", " AE", " AG", " Alfa", " Alpha", " Archer", " Army", " Atlas", " Azul", 
			" B", " BA", " BAMF", " BE", " BI", " BM", " BMF", " BMW", " BP", " BW", " BX", " BYE", " Badass", " Black", " Blue", " Boosted", " Bowman", " Bravo", " Bronze", " Brvo", 
			" C", " CD", " CM", " CP", " CR", " CV", " CX", " CZ", " CZ17", " Calliente", " Caravan", " Cavalry", " Champion", " Char", " Chariot", " Charles", " Charlie", " Checkered", " Class", " Classic", " Courier", " Custom", 
			" D", " DEEZ", " DG", " DP", " DT", " DV", " DZ", " Del", " Delta", " Delux", " Deluxe", " Destroyer", " Dlta", " Drifter", 
			" E", " E Class", " E-V", " EM", " EN", " EP", " ES", " ESP", " ET", " EV", " EX", " EXP", " Echo", " Ecko", " Eco", " Eko", " Elite", " Experimental", " Explorer", 
			" F", " FG", " FN", " FNG", " FRAG", " FRG", " FX", " Facade", " Fake", " Fanatic", " Featherlight", " Ferry", " Flashy", " Floater", " For Sale", " Foxtrot", " Ftl", " Fxtr", " Fxtrt", 
			" G", " GO", " GOVT", " GT", " GTX", " GX", " Galley", " Gamma", " Gma", " Gold", " Gov", " Government", " Green", 
			" H", " HA", " HI", " HV", " HVY", " HWY", " Heavy", " Hero", " Hotel", " Htl", 
			" I", " IG", " II", " III", " IK", " IKR", " IR", " IR8M8", " IT", " ITME", " IV", " IX", " IY", " Idg", " Impulse", " Indigo", 
			" J", " JATO", " JB", " JBLK", " JET", " JK", " JKJK", " JP", " JPOP", " JR", " JROC", " JTO", " JV", " Jr", 
			" K", " KK", " KLM", " KO", " KP", " KPN", " KPOP", " KPS", " KQ", " KQED", " KV", " KX", " Knockoff", 
			" L", " L8RG8R", " LGT", " LP", " LPN", " LRP", " LVL", " LVN", " LVS", " LX", " Legendary", " Light", " Lightweight", " Lux", " Luxury", 
			" M", " M Class", " MAN", " MB", " MED", " MEN", " MG", " MI", " MIL", " MN", " MO", " MON", " MR", " MS", " MSFT", " MU", " MX", " MY", " MZ", " Mapping", " Master Class", " Mb", " Med", " Medium", " Merch", " Merchant", " Merry", " Mk.I", " Mk.II", " Mk.III", " Model 1", " Model 2", " Model 3", " Model 4", " Model A", " Model B", " Model C", " Modern", " Mr", 
			" N", " NO", " NOB", " NOOB", " NOX", " NS", " NUB", " Negro", " Night", " Nix", " No", " Nox", " Nuclear", 
			" O", " OBO", " OD", " ODO", " OE", " OO", " OP", " OV", " Official", " Orbiter", 
			" P", " PDP", " PDW", " PX", " Plat", " Platinum", " Premium", " Pristine", " Purple", 
			" Q", 
			" R", " RFD", " Radeon", " Red", " Reinforced", " Revamped", " Revived", " Rojo", " Royal", " Royalty", 
			" S", " S Class", " SCO", " Salvaged", " Scavenger", " Scout", " Shielded", " Silver", " Sketchy", " Skirmisher", " Slinger", " Solar", " Spinoff", " Superlight", 
			" T", " TI", " Tanking", " Taxi", " Trans", " Transport Class", " Turbocharged", 
			" U", " Unregistered", 
			" V", " V1", " V2", " V3", " V4", " V5", " VII", " Vetted", " Vintage", 
			" W", " Wanderer", 
			" X", " Xplor", 
			" Y", " Yellow", 
			" Z", 
			" v2", " v3", " v4", " v5", " v6"
		};

		/*
		public static void sortIds() {
			TreeSet<string> sorted = new TreeSet<string>();
			for(int i = 0; i < ids.Length; i++) {
				sorted.Add(ids[i]);
			}
			
			StringBuilder sb = new StringBuilder();
			sb.Append("{\n");
			
			char c = sorted.first().charAt(0);
			foreach(string id in sorted) {
				if(c == id.charAt(0)) {
					sb.Append("\"").Append(id).Append("\", ");
				} else {
					c = id.charAt(0);
					sb.Append("\n\"").Append(id).Append("\", ");
				}
			}
			sb.deleteCharAt(sb.Length - 1);
			sb.deleteCharAt(sb.length() - 1);
			
			sb.Append("\n};");

			Debug.Log (sb.ToString());
		}
		*/
		/*
		public static void sortIdentifiers() {
			TreeSet<string> sorted = new TreeSet<string>();
			for(int i = 0; i < identifiers.Length; i++) {
				sorted.Add(identifiers[i]);
			}
			
			StringBuilder sb = new StringBuilder();
			sb.Append("{\n");
			
			char c = sorted.First().charAt(1);
			foreach(string id in sorted) {
				if(c == id.charAt(1)) {
					sb.Append("\"").Append(id).Append("\", ");
				} else {
					c = id.charAt(1);
					sb.Append("\n\"").Append(id).Append("\", ");
				}
			}
			sb.deleteCharAt(sb.Length - 1);
			sb.deleteCharAt(sb.Length - 1);
			
			sb.Append("\n};");

			Debug.Log(sb.ToString());
		}
		*/
	}
}