using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaitAndEat.Model
{
    class Party : ModelBase
    {
        public static String NotifiedNo = "No";
        public static String NotifiedYes = "Yes";

        public Party()
        {
            this._notified = NotifiedNo;
        }

        public Party(string name, string phone, string size) 
        {
            this._name = name;
            this._phone = phone;
            this._size = size;
            this._notified = NotifiedNo;
        }

        private bool _done = false;
        public bool done
        {
            set
            {
                if (value != _done)
                {
                    _done = value;
                    NotifyPropertyChanged("done");
                }
            }
            get
            {
                return _done;
            }
        }

        private string _id = null;
        public string id
        {
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("id");
                }
            }
            get
            {
                return _id;
            }
        }

        private string _name = null;
        public string name
        {
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("name");
                }
            }
            get
            {
                return _name;
            }
        }

        private string _notified = null;
        public string notified
        {
            set
            {
                if (value != _notified)
                {
                    _notified = value;
                    NotifyPropertyChanged("notified");
                }
            }
            get
            {
                return _notified;
            }
        }

        private string _phone = null;
        public string phone
        {
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    NotifyPropertyChanged("phone");
                }
            }
            get
            {
                return _phone;
            }
        }

        private string _size = null;
        public string size
        {
            set
            {
                if (value != _size)
                {
                    _size = value;
                    NotifyPropertyChanged("size");
                }
            }
            get
            {
                return _size;
            }
        }

        public override int GetHashCode()
        {
            int result = _id != null ? _id.GetHashCode() : 0;
            result = 29 * result + _name != null ? _name.GetHashCode() : 0;
            result = 29 * result + _notified != null ? _notified.GetHashCode() : 0;
            result = 29 * result + _phone != null ? _phone.GetHashCode() : 0;
            result = 29 * result + _size != null ? _size.GetHashCode() : 0;
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj) return true;
            Party other = obj as Party;
            if (other == null) return false;
            if (!Equals(_done, other._done)) return false;
            if (!Equals(_id, other._id)) return false;
            if (!Equals(_name, other._name)) return false;
            if (!Equals(_notified, other._notified)) return false;
            if (!Equals(_phone, other._phone)) return false;
            if (!Equals(_size, other._size)) return false;
            return true;
        }
    }
}
