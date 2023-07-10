using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG_ServerUI
{
    public class Raceparticipant
    {
        private int _id;
        public int Id { get { return _id; } set { _id = value; } }
        private string _hash;
        public string Hash { get { return _hash; } set { _hash = value; } }
        private string _name;
        public string? Name { get { return _name; } set { _name = value; } }
        private bool _hasstarted;
        public bool Hasstarted { get { return _hasstarted; } set { _hasstarted = value; } }
        private bool _hasfinished;
        public bool Hasfinished { get { return _hasfinished; } set { _hasfinished = value; } }
        private float _racetime;
        public float Racetime { get { return _racetime; } set { _racetime = value; } }

        private float _flightime;
        public float Flighttime { get { return (int)_flightime; } set { _flightime = value * 100; } }
        private int _points;
        public int Points { get { return _points; } set { _points = value; } }

        public Raceparticipant() 
        {
            Hasstarted = false;
            Hasfinished = false;
            Points = 0;
            Racetime = 0;
            Flighttime = 0;
            Hash = "";
        }

    }
}
