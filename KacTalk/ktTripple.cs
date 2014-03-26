using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktTripple : ktIntObj
    {
        // Constructors
        public ktTripple(bool _One, bool _Two)
            : base("ktTripple", 0)
        {
            One = _One;
            Two = _Two;
        }
        public ktTripple()
            : base("ktTripple", 0)
        {
            One = false;
            Two = false;
        }
        public ktTripple(int Value)
            : base("ktTripple", 0)
        {
            SetValue(Value);
        }

        public void SetValue(bool _One, bool _Two)
        {
            One = _One;
            Two = _Two;
        }
        public void SetValue()
        {
            SetValue(false, false);
        }

        public void SetValue(char Val)
        {
            if (Val % 2 == 1)
                One = true;
            else
                One = false;
            if (Val >= 2)
                Two = true;
            else
                Two = false;
        }

        public void SetValue(int Val)
        {
            if (Val % 2 == 1)
                One = true;
            else
                One = false;
            if (Val >= 2)
                Two = true;
            else
                Two = false;
        }

        public void GetValue(out bool _One, out bool _Two)
        {
            _One = One;
            _Two = Two;
        }

        public char GetValue()
        {
            int Ret = 0;

            if (One) Ret++;
            if (Two) Ret += 2;

            return (char)Ret;
        }

        public override int GetHashCode()
        {
            return GetValue();
        }
        public override bool Equals(object Obj)
        {
            if (Obj == null) return false;

            if (this.GetType() != Obj.GetType()) return false;

            // safe because of the GetType check
            ktTripple Tr = (ktTripple)Obj;

            return this.GetValue() == Tr.GetValue();
        }

        public static bool operator !(ktTripple Tr)
        {
            return (Tr == 0);
        }

        public static char operator +(ktTripple Tr, char Value)
        {
            return (char)((int)Tr.GetValue() + (int)Value);
        }
        public static char operator +(ktTripple Tr1, ktTripple Tr2)
        {
            return (char)((int)Tr1.GetValue() + (int)Tr2.GetValue());
        }
        public static int operator +(ktTripple Tr, int Value)
        {
            return Tr.GetValue() + Value;
        }
        public static ktTripple operator ++(ktTripple Tr)
        {
            Tr.SetValue(Tr.GetValue() + 1);
            return Tr;
        }

        public static char operator -(ktTripple Tr, char Value)
        {
            return (char)((int)Tr.GetValue() - (int)Value);
        }
        public static char operator -(ktTripple Tr1, ktTripple Tr2)
        {
            return (char)((int)Tr1.GetValue() - (int)Tr2.GetValue());
        }
        public static char operator -(ktTripple Tr)
        {
            return (char)(0 - (int)Tr.GetValue());
        }
        public static ktTripple operator --(ktTripple Tr)
        {
            Tr.SetValue(Tr.GetValue() - 1);
            return Tr;
        }

        public static implicit operator ktTripple(int Value)
        {
            return new ktTripple(Value);
        }
        public static implicit operator ktTripple(char Value)
        {
            return new ktTripple(Value);
        }
        public static implicit operator ktTripple(bool Value)
        {
            return new ktTripple(Value, false);
        }
        public static implicit operator int(ktTripple Value)
        {
            return (int)Value.GetValue();
        }
        public static implicit operator char(ktTripple Value)
        {
            return Value.GetValue();
        }
        public static implicit operator bool(ktTripple Value)
        {
            return (Value.GetValue() != 0);
        }

        public static bool operator ==(ktTripple Tr, ktTripple Tr2)
        {
            return (Tr.GetValue() == Tr2.GetValue());
        }
        public static bool operator ==(ktTripple Tr, int Value)
        {
            return (((int)Tr.GetValue()) == Value);
        }
        public static bool operator ==(ktTripple Tr, char Value)
        {
            return (Tr.GetValue() == Value);
        }

        public static bool operator !=(ktTripple Tr, ktTripple Tr2)
        {
            return (Tr.GetValue() != Tr2.GetValue());
        }
        public static bool operator !=(ktTripple Tr, int Value)
        {
            return (((int)Tr.GetValue()) != Value);
        }
        public static bool operator !=(ktTripple Tr, char Value)
        {
            return (Tr.GetValue() != Value);
        }

        public static bool operator >(ktTripple Tr, ktTripple Tr2)
        {
            return (Tr.GetValue() > Tr2.GetValue());
        }
        public static bool operator >(ktTripple Tr, int Value)
        {
            return (((int)Tr.GetValue()) > Value);
        }
        public static bool operator >(ktTripple Tr, char Value)
        {
            return (Tr.GetValue() > Value);
        }

        public static bool operator <(ktTripple Tr, ktTripple Tr2)
        {
            return (Tr.GetValue() < Tr2.GetValue());
        }
        public static bool operator <(ktTripple Tr, int Value)
        {
            return (((int)Tr.GetValue()) < Value);
        }
        public static bool operator <(ktTripple Tr, char Value)
        {
            return (Tr.GetValue() < Value);
        }

        public static bool operator <=(ktTripple Tr, ktTripple Tr2)
        {
            return (Tr.GetValue() <= Tr2.GetValue());
        }
        public static bool operator <=(ktTripple Tr, int Value)
        {
            return (((int)Tr.GetValue()) <= Value);
        }
        public static bool operator <=(ktTripple Tr, char Value)
        {
            return (Tr.GetValue() <= Value);
        }

        public static bool operator >=(ktTripple Tr, ktTripple Tr2)
        {
            return (Tr.GetValue() >= Tr2.GetValue());
        }
        public static bool operator >=(ktTripple Tr, int Value)
        {
            return (((int)Tr.GetValue()) >= Value);
        }
        public static bool operator >=(ktTripple Tr, char Value)
        {
            return (Tr.GetValue() >= Value);
        }

        private bool One;
        private bool Two;
    }
}
