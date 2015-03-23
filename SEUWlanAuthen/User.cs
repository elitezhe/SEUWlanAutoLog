using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SEUWlanAuthen
{
    class User
    {
        public string StuID { get; set; }
        public string Pwd { get; set; }

        public User() { }
        public User(string stuId, string Pwd)
        {
            this.StuID = stuId;
            this.Pwd = Pwd;
        }
        public User(User aSeuUser)
        {
            this.StuID = aSeuUser.StuID;
            this.Pwd = aSeuUser.Pwd;
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
        public void DeSerialize(string jsonStr)
        {
            User userFromJson = JsonConvert.DeserializeObject<User>(jsonStr);
            this.StuID = userFromJson.StuID;
            this.Pwd = userFromJson.Pwd;
        }
        public bool IsEqual (User anotherUser)
        {
            if(this.StuID == anotherUser.StuID && this.Pwd == anotherUser.Pwd)
                return true;
            else
                return false;
        }
    }
}
