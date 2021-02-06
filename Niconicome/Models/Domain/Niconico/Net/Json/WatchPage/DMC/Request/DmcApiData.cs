using System.Collections.Generic;

namespace Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request
{
    public class DmcPostData
    {
        public Session Session { get; set; } = new();

        public static DmcPostData GetInstance()
        {
            return new DmcPostData();
        }
    }

    public class Session
    {
        public string? Recipe_id { get; set; }
        public string? Content_id { get; set; }
        public string? Content_type { get; set; }
        public List<Content_Src_Id_Sets> Content_src_id_sets { get; set; } = new();
        public string? Timing_constraint { get; set; }
        public Keep_Method Keep_method { get; set; } = new();
        public Protocol Protocol { get; set; } = new();
        public string Content_uri { get; set; } = string.Empty;
        public Session_Operation_Auth Session_operation_auth { get; set; } = new();
        public Content_Auth Content_auth { get; set; } = new();
        public Client_Info Client_info { get; set; } = new();
        public float Priority { get; set; }
    }

    public class Keep_Method
    {
        public Heartbeat Heartbeat { get; set; } = new();
    }

    public class Heartbeat
    {
        public int Lifetime { get; set; }
    }

    public class Protocol
    {
        public string? Name { get; set; }
        public Parameters Parameters { get; set; } = new();
    }

    public class Parameters
    {
        public Http_Parameters Http_parameters { get; set; } = new();
    }

    public class Http_Parameters
    {
        public Parameters1 Parameters { get; set; } = new();
    }

    public class Parameters1
    {
        public Hls_Parameters Hls_parameters { get; set; } = new();
    }

    public class Hls_Parameters
    {
        public string? Use_well_known_port { get; set; }
        public string? Use_ssl { get; set; }
        public string? Transfer_preset { get; set; }
        public int Segment_duration { get; set; }
    }

    public class Session_Operation_Auth
    {
        public Session_Operation_Auth_By_Signature Session_operation_auth_by_signature { get; set; } = new();
    }

    public class Session_Operation_Auth_By_Signature
    {
        public string? Token { get; set; }
        public string? Signature { get; set; }
    }

    public class Content_Auth
    {
        public string? Auth_type { get; set; }
        public int Content_key_timeout { get; set; }
        public string? Service_id { get; set; }
        public string? Service_user_id { get; set; }
    }

    public class Client_Info
    {
        public string? Player_id { get; set; }
    }

    public class Content_Src_Id_Sets
    {
        public List<Content_Src_Ids> Content_src_ids { get; set; } = new();
    }

    public class Content_Src_Ids
    {
        public Src_Id_To_Mux Src_id_to_mux { get; set; } = new();
    }

    public class Src_Id_To_Mux
    {
        public List<string> Video_src_ids { get; set; } = new();
        public List<string> Audio_src_ids { get; set; } = new();
    }

}
