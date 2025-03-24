using System.Diagnostics.CodeAnalysis;

namespace FantasyFootball.Shared
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    [System.Xml.Serialization.XmlRoot(Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng", IsNullable = false)]
    [ExcludeFromCodeCoverage]
    public partial class fantasy_content
    {

        private fantasy_contentPlayers playersField;

        private string langField;

        private string uriField;

        private string timeField;

        private string copyrightField;

        private int refresh_rateField;

        /// <remarks/>
        public fantasy_contentPlayers players
        {
            get
            {
                return playersField;
            }
            set
            {
                playersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return langField;
            }
            set
            {
                langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.yahooapis.com/v1/base.rng")]
        public string uri
        {
            get
            {
                return uriField;
            }
            set
            {
                uriField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string time
        {
            get
            {
                return timeField;
            }
            set
            {
                timeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string copyright
        {
            get
            {
                return copyrightField;
            }
            set
            {
                copyrightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public int refresh_rate
        {
            get
            {
                return refresh_rateField;
            }
            set
            {
                refresh_rateField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayers
    {

        private fantasy_contentPlayersPlayer[] playerField;

        private int countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("player")]
        public fantasy_contentPlayersPlayer[] player
        {
            get
            {
                return playerField;
            }
            set
            {
                playerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public int count
        {
            get
            {
                return countField;
            }
            set
            {
                countField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayersPlayer
    {

        private string player_keyField;

        private int player_idField;

        private fantasy_contentPlayersPlayerName nameField;

        private string injury_noteField;

        private string editorial_player_keyField;

        private string editorial_team_keyField;

        private string editorial_team_full_nameField;

        private string editorial_team_abbrField;

        private fantasy_contentPlayersPlayerBye_weeks bye_weeksField;

        private string uniform_numberField;

        private string display_positionField;

        private fantasy_contentPlayersPlayerHeadshot headshotField;

        private string image_urlField;

        private int is_undroppableField;

        private string position_typeField;

        private fantasy_contentPlayersPlayerEligible_positions eligible_positionsField;

        private int has_player_notesField;

        private bool has_player_notesFieldSpecified;

        private fantasy_contentPlayersPlayerPlayer_stats player_statsField;

        /// <remarks/>
        public string player_key
        {
            get
            {
                return player_keyField;
            }
            set
            {
                player_keyField = value;
            }
        }

        /// <remarks/>
        public int player_id
        {
            get
            {
                return player_idField;
            }
            set
            {
                player_idField = value;
            }
        }

        /// <remarks/>
        public fantasy_contentPlayersPlayerName name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }

        /// <remarks/>
        public string injury_note
        {
            get
            {
                return injury_noteField;
            }
            set
            {
                injury_noteField = value;
            }
        }

        /// <remarks/>
        public string editorial_player_key
        {
            get
            {
                return editorial_player_keyField;
            }
            set
            {
                editorial_player_keyField = value;
            }
        }

        /// <remarks/>
        public string editorial_team_key
        {
            get
            {
                return editorial_team_keyField;
            }
            set
            {
                editorial_team_keyField = value;
            }
        }

        /// <remarks/>
        public string editorial_team_full_name
        {
            get
            {
                return editorial_team_full_nameField;
            }
            set
            {
                editorial_team_full_nameField = value;
            }
        }

        /// <remarks/>
        public string editorial_team_abbr
        {
            get
            {
                return editorial_team_abbrField;
            }
            set
            {
                editorial_team_abbrField = value;
            }
        }

        /// <remarks/>
        public fantasy_contentPlayersPlayerBye_weeks bye_weeks
        {
            get
            {
                return bye_weeksField;
            }
            set
            {
                bye_weeksField = value;
            }
        }

        /// <remarks/>
        public string uniform_number
        {
            get
            {
                return uniform_numberField;
            }
            set
            {
                uniform_numberField = value;
            }
        }

        /// <remarks/>
        public string display_position
        {
            get
            {
                return display_positionField;
            }
            set
            {
                display_positionField = value;
            }
        }

        /// <remarks/>
        public fantasy_contentPlayersPlayerHeadshot headshot
        {
            get
            {
                return headshotField;
            }
            set
            {
                headshotField = value;
            }
        }

        /// <remarks/>
        public string image_url
        {
            get
            {
                return image_urlField;
            }
            set
            {
                image_urlField = value;
            }
        }

        /// <remarks/>
        public int is_undroppable
        {
            get
            {
                return is_undroppableField;
            }
            set
            {
                is_undroppableField = value;
            }
        }

        /// <remarks/>
        public string position_type
        {
            get
            {
                return position_typeField;
            }
            set
            {
                position_typeField = value;
            }
        }

        /// <remarks/>
        public fantasy_contentPlayersPlayerEligible_positions eligible_positions
        {
            get
            {
                return eligible_positionsField;
            }
            set
            {
                eligible_positionsField = value;
            }
        }

        /// <remarks/>
        public int has_player_notes
        {
            get
            {
                return has_player_notesField;
            }
            set
            {
                has_player_notesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnore()]
        public bool has_player_notesSpecified
        {
            get
            {
                return has_player_notesFieldSpecified;
            }
            set
            {
                has_player_notesFieldSpecified = value;
            }
        }

        /// <remarks/>
        public fantasy_contentPlayersPlayerPlayer_stats player_stats
        {
            get
            {
                return player_statsField;
            }
            set
            {
                player_statsField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayersPlayerName
    {

        private string fullField;

        private string firstField;

        private string lastField;

        private string ascii_firstField;

        private string ascii_lastField;

        /// <remarks/>
        public string full
        {
            get
            {
                return fullField;
            }
            set
            {
                fullField = value;
            }
        }

        /// <remarks/>
        public string first
        {
            get
            {
                return firstField;
            }
            set
            {
                firstField = value;
            }
        }

        /// <remarks/>
        public string last
        {
            get
            {
                return lastField;
            }
            set
            {
                lastField = value;
            }
        }

        /// <remarks/>
        public string ascii_first
        {
            get
            {
                return ascii_firstField;
            }
            set
            {
                ascii_firstField = value;
            }
        }

        /// <remarks/>
        public string ascii_last
        {
            get
            {
                return ascii_lastField;
            }
            set
            {
                ascii_lastField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayersPlayerBye_weeks
    {

        private int weekField;

        /// <remarks/>
        public int week
        {
            get
            {
                return weekField;
            }
            set
            {
                weekField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayersPlayerHeadshot
    {

        private string urlField;

        private string sizeField;

        /// <remarks/>
        public string url
        {
            get
            {
                return urlField;
            }
            set
            {
                urlField = value;
            }
        }

        /// <remarks/>
        public string size
        {
            get
            {
                return sizeField;
            }
            set
            {
                sizeField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayersPlayerEligible_positions
    {

        private string positionField;

        /// <remarks/>
        public string position
        {
            get
            {
                return positionField;
            }
            set
            {
                positionField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayersPlayerPlayer_stats
    {

        private string coverage_typeField;

        private int weekField;

        private fantasy_contentPlayersPlayerPlayer_statsStat[] statsField;

        /// <remarks/>
        public string coverage_type
        {
            get
            {
                return coverage_typeField;
            }
            set
            {
                coverage_typeField = value;
            }
        }

        /// <remarks/>
        public int week
        {
            get
            {
                return weekField;
            }
            set
            {
                weekField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem("stat", IsNullable = false)]
        public fantasy_contentPlayersPlayerPlayer_statsStat[] stats
        {
            get
            {
                return statsField;
            }
            set
            {
                statsField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class fantasy_contentPlayersPlayerPlayer_statsStat
    {

        private string stat_idField;

        private string valueField;

        /// <remarks/>
        public string stat_id
        {
            get
            {
                return stat_idField;
            }
            set
            {
                stat_idField = value;
            }
        }

        /// <remarks/>
        public string value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }
}