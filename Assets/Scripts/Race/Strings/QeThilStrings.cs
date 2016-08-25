namespace Races {
    class QeThilStrings {
        // Base pattern = Qw/Qe, Ith/Ithe, Th/The/Tha
        // Lots of Th, Qw, Qe, ("k")
        // No G, J, K, M, P, Y, Z, hard T

        // Masculine patterns = or, ell, an, ul, vo
        public static string[] mascBase = new string[] {
            "Ithan", "Ithell", "Ithevo", "Ithor", "Ithul", "Ithvo",
            "Qean", "Qeor", "Qethan", "Qethell", "Qethor", "Qethul", "Qethvo", "Qeul", "Qevo", "Qwan", "Qwell", "Qwethan", "Qwethell", "Qwethor", "Qwethul", "Qwethvo", "Qwor", "Qwul",
            "Thaan", "Thaell", "Than", "Thaor", "Thavo", "Thell", "Theor", "Thevo", "Thoqan", "Thoqean", "Thoqell", "Thoqeor", "Thoqeul", "Thoqevo", "Thoqor", "Thoqul", "Thoqvo", "Thor", "Thul"
        };

        // Feminine patterns = ee, al, vr, il, sa
        public static string[] femBase = new string[] {
            "Ithal", "Itheal", "Ithee", "Itheil", "Ithesa", "Ithevr", "Ithil", "Ithsa", "Ithvr",
            "Qesa", "Qethal", "Qethee", "Qethevr", "Qethil", "Qethsa", "Qethvr", "Qevr", "Qwal", "Qwee", "Qwethual", "Qwethuee", "Qwethuil", "Qwethusa", "Qwethuvr", "Qwil",
            "Thaee", "Thail", "Thal", "Thasa", "Thavr", "Thee", "Theil", "Thesa", "Thevr", "Thil", "Thuqal", "Thuqeal", "Thuqee", "Thuqeil", "Thuqesa", "Thuqevr", "Thuqil", "Thuqsa", "Thuqvr", "Thuqwee"
        };

        public static string[] mascEnds = new string[] {
            "An", "Aan",
            "Bo", "Bol", "Bool",
            "Cwo", "Cwoo",
            "Doth", "Do", "Dwo",
            "Ethe", "Eth", "Etho",
            "Fle", "Flo", "Flol",
            "Hox", "Ho", "Hoth",
            "Lethe", "Lo", "Loo", "Lan", "Laan",
            "Nle", "No", "Noo", "Nan",
            "Oth", "Othe", "Ox",
            "Qe", "Qeth", "Qo",
            "Rethe", "Rox",
            "Sethe", "Sox",
            "Tho", "Toth",
            "Uvo", "Uvoth",
            "Vo", "Voth", "Vr",
            "Wan", "Woth", "Wo", "Wvr",
            "X"
        };

        public static string[] femEnds = new string[] {
            "Aa", "A", "Athu", "Aethu",
            "Bu", "Bul", "Bath", "Baeth",
            "Caeth", "Cu", "Caa", "Ca", "Cethu", 
            "Daeth", "Daa", "Dath", "Dethu", 
            "Ethu", "Eth", "Etha",
            "Fil", "Fath", "Fethu",
            "Hux", "Hath", "Haa", "Hu",
            "Lu", "Lethu", "Laa", "La",
            "Naeth", "Nil", "Nu", "Nuwa",
            "Qu", "Qeth",
            "Ru", "Rux", "Rethu", "Raethu",
            "Sil", "Su", "Sux", "Sewa",
            "Thaeth", "Thu",
            "Uvux", "Uvethu",
            "Vil", "Vethu",
            "Wa", "Wu", "Waa", "Wul"
        };
        public static string detailedDescription = "The Qe'Thil society is a highly advanced, competitive, paranoid, and secretive race. They are a peaceful race that finds no honor in murder and war, instead they thrive in a one-upmanship environment where the greatest 'improvers' recieve great fame and respect. Their competitive culture allowed them to be one of the first known cultures to establish faster than light travel and their primary leaders are some of the wisest individuals in the universe. Their living conditions seem almost utopian, but they are very critical of 'inferior' outsiders. Some exceptional outsiders have been able to earn membership into the society by improving something held dear to the Qe'Thil.";

        public static string shortDescription = "The Qe'Thil are a peaceful race of brave explorers. Qe'Thil are under a meter tall and are typically described as slender bears.";
    }
}
