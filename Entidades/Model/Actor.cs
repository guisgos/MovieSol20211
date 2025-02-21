﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.Model


{
    public class Actor

    {

        public int ActorId { get; set; }
        public String Name { get; set; }
        public DateTime DateBirth { get; set; }

        public virtual ICollection<ActorMovie> Characters { get; set; }

    }
}
