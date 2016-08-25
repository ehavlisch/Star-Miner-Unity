using System;
using Util;

namespace Races {
    public class Aug : Race {

        public const string NAME = "Augmented Human";

        public override string generateName(bool masculine) {
            return Utils.getMember(AugStrings.names) + " " + Utils.getMember(AugStrings.designations);
        }

        public override int getDefaultPlayerRelationship() {
            return 5000;
        }

        public override int getDefaultRelationship(string name) {
            switch (name) {
                default:
                    return 5000;
            }
        }

        public override string getDetailedDescription() {
            return "An Aug, short for augmented, is a member of a technologically advanced society. The Aug society functions as a democratic hive mind. Each individual Aug has a limited notion of individualism because their memories and thoughts are publically available to the rest of the race. Augs are grown from human embryos in incubators and are 'birthed' when all major organs have been replaced with long lasting cybernetic organs. A young Aug will be educated by replaying the memories and experiences of the rest of the race. Once the Aug has reviewed the entirety of the race's memories, they are free to live out their life as they please. Each aug can live for centuries or can die due to a faulty component at a much younger age. Augs still considered themselves humans until they developed an circulatory system that could function in the vaccuum of space. Once Earth became inhospitable for humans, the Homo Augmentus was officially created as many Augs remained on Earth.";
        }

        public override string getMemberName() {
            return "Aug";
        }

        public override string getName() {
            return NAME;
        }

        public override string getPluralMemberName() {
            return "Augs";
        }

        public override string getPluralName() {
            return "Augmented Humans";
        }

        public override string getShortDescription() {
            return "A human that has undergone extensive cybernetic enhancements.";
        }
    }
}