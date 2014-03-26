using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KacTalk;

namespace ktMainLib
{
    public class ktInt : ktClass
    {
        /// <summary>
        /// Create a new ktInt
        /// </summary>
        /// <param name="value">value to initially give the ktInt</param>
        public ktInt( int value ) : base("ktInt")
        {
            m_value = value;
        }
        /// <summary>
        /// Create a new ktInt
        /// </summary>
        public ktInt() : this(0) { }

        public ktInt(ktInt val)
            : base("ktInt")
        {
            // TODO: Complete member initialization
            m_value = val.m_value;
            m_HardType = val.m_HardType;
            m_IsClass = val.m_IsClass;
            m_Parent = val.m_Parent;
            m_IsConstant = val.m_IsConstant;
        }

        /// <summary>
        /// Run a method
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        public override ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            //ktDebug.Log("ktInt::" + Name);

            // Check the name of the method to call and call appropriate method
            switch (Name.AsLower())
            {
                case "_add":
                case "op+":
                case "operator+":
                    {
                        Value = _Add(Arguments);
                        break;
                    }
                case "add":
                case "op+=":
                case "operator+=":
                case "_append":
                    {
                        CheckIfConstant(Name);
                        Value = Add(Arguments);
                        break;
                    }
                case "increase":
                case "op++":
                case "operator++":
                case "_increase":
                    {
                        CheckIfConstant(Name);
                        Value = Increase();
                        break;
                    }
                case "_subtract":
                case "op-":
                case "operator-":
                    {
                        Value = _Subtract(Arguments);
                        break;
                    }
                case "op-=":
                case "operator-=":
                case "subtract":
                    {
                        CheckIfConstant(Name);
                        Value = Subtract(Arguments);
                        break;
                    }
                case "decrease":
                case "op--":
                case "operator--":
                case "_decrease":
                    {
                        CheckIfConstant(Name);
                        Value = Decrease();
                        break;
                    }
                case "_multiply":
                case "_times":
                case "op*":
                case "operator*":
                    {
                        Value = _Multiply(Arguments);
                        break;
                    }
                case "multiply":
                case "times":
                case "op*=":
                case "operator*=":
                    {
                        CheckIfConstant(Name);
                        Value = Multiply(Arguments);
                        break;
                    }
                case "_divide":
                case "op/":
                case "operator/":
                    {
                        Value = _Divide(Arguments);
                        break;
                    }
                case "op/=":
                case "operator/=":
                case "divide":
                    {
                        CheckIfConstant(Name);
                        Value = Divide(Arguments);
                        break;
                    }
                case "_mod":
                case "_modulus":
                case "op%":
                case "operator%":
                    {
                        Value = _Modulus(Arguments);
                        break;
                    }
                case "op%=":
                case "operator%=":
                case "modulus":
                    {
                        CheckIfConstant(Name);
                        Value = Modulus(Arguments);
                        break;
                    }
                case "_power":
                case "_pow":
                case "op**":
                case "operator**":
                    {
                        Value = _Power(Arguments);
                        break;
                    }
                case "op^":
                case "operator^":
                    {
                        if (kacTalk.Main.MathMode)
                        {
                            Value = _Power(Arguments);
                        }
                        else
                        {
                            Value = _ExclusiveOr(Arguments);
                        }
                        break;
                    }
                case "op^=":
                case "operator^=":
                    {
                        CheckIfConstant(Name);
                        if (kacTalk.Main.MathMode)
                        {
                            Value = Power(Arguments);
                        }
                        else
                        {
                            Value = ExclusiveOr(Arguments);
                        }
                        break;
                    }
                case "power":
                case "pow":
                    {
                        CheckIfConstant(Name);
                        Value = Power(Arguments);
                        break;
                    }
                case "_assign":
                case "op=":
                case "operator=":
                case "assign":
                    {
                        CheckIfConstant(Name);
                        Value = Assign(Arguments);
                        break;
                    }
                case "tobase":
                    {
                        Value = ToBase(Arguments);
                        break;
                    }
                case "tobinary":
                    {
                        Value = _ToBase(2);
                        break;
                    }
                /*case "toint":
                    {
                        Value = new ktValue("return", "ktInt", new ktInt(m_value), true, true);
                        break;
                    }*/
                case "tobool":
                    {
                        Value = _ToBool();
                        break;
                    }
                case "tooct":
                    {
                        Value = _ToBase(8);
                        break;
                    }
                case "tohex":
                    {
                        Value = _ToBase(16);
                        break;
                    }
                case "frombase":
                    {
                        Value = FromBase(Arguments);
                        break;
                    }
                case "frombinary":
                    {
                        Value = FromBase(Arguments, 2);
                        break;
                    }
                case "fromoct":
                    {
                        Value = FromBase(Arguments, 8);
                        break;
                    }
                case "fromhex":
                    {
                        Value = FromBase(Arguments, 16);
                        break;
                    }
                case ">":
                case "op>":
                case "operator>":
                case "mt":
                case "gt":
                case "greater":
                case "greaterthan":
                case "more":
                case "morethan":
                case "isgreater":
                case "isgreaterthan":
                case "ismore":
                case "ismorethan":
                case ">=":
                case "op>=":
                case "operator>=":
                case "mte":
                case "gte":
                case "greaterorequal":
                case "greaterthanorequal":
                case "moreorequal":
                case "morethanorequal":
                case "isgreaterorequal":
                case "isgreaterthanorequal":
                case "ismoreorequal":
                case "ismorethanorequal":
                case "<":
                case "op<":
                case "operator<":
                case "lt":
                case "less":
                case "lessthan":
                case "isless":
                case "islessthan":
                case "<=":
                case "op<=":
                case "operator<=":
                case "lte":
                case "lessorequal":
                case "lessthanorequal":
                case "islessorequal":
                case "islessthanorequal":
                case "<>":
                case "!=":
                case "op<>":
                case "op!=":
                case "operator<>":
                case "operator!=":
                case "ne":
                case "isnotequal":
                case "notequal":
                case "==":
                case "op==":
                case "operator==":
                case "isequal":
                case "equal":
                case "eq":
                case "compare":
                    {
                        Value = _Compare(Name,Arguments);
                        break;
                    }
                default:
                    {
                        throw new ktError("Couldn't find the method '" +
                                          Name + "' in class '" + m_Name + "'.", ktERR._404);
                    }
            }

            // Done...
            return Value;
        }

        /// <summary>
        /// Convert from one base to decimal
        /// </summary>
        /// <param name="strVal">The value to convert</param>
        /// <param name="frombase">The base to convert to from</param>
        /// <returns></returns>
        public ktValue _FromBase(ktString strVal, int frombase = 2)
        {
            // Convert the given value
            int intVal = Convert.ToInt32(strVal, frombase);

            // Is this instance treated as a class
            if (this.m_IsClass)
            {
                // Create a new ktInt and wrap it in a ktValue
                return new ktValue("return", "ktInt", new ktInt(intVal), true, true);
            }
            // this is an object
            else
            {
                // Set the value
                m_value = intVal;
                // Return this object wrapped in a ktValue
                return new ktValue(this.Name, "ktInt", this, m_HardType, false);
            }
        }

        /// <summary>
        /// Convert from one base to decimal
        /// </summary>
        /// <param name="Arguments">A list of arguments to convert to decimal (only uses the first one)</param>
        /// <param name="frombase">The base to convert from</param>
        /// <returns></returns>
        public ktValue FromBase(ktList Arguments, int frombase = 2)
        {
            ktValue Arg = ktValue.Null;
            string val = "";

            // Did we get any arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Not enough arguments for ktInt::FromBase(??," + frombase + ")!", ktERR.NOTDEF);
            }

            // Get the first argument and convert it to a string
            Arg = (ktValue)Arguments.First.Node.Value;
            val = Arg.ToString();

            // Convert
            return _FromBase(val, frombase);
        }

        /// <summary>
        /// Convert from one base to decimal
        /// </summary>
        /// <param name="Arguments">A list of arguments (Arg1: The value to convert, Arg2: The base to convert from)</param>
        /// <returns></returns>
        public ktValue FromBase(ktList Arguments)
        {
            ktValue Arg = ktValue.Null;
            string val = "";
            int frombase = 2;

            // Did we get any arguments?
            if (Arguments.IsEmpty())
            {
                throw new ktError("Not enough arguments for ktInt::FromBase()!", ktERR.NOTDEF);
            }
            // Did we get enough arguments?
            else if (Arguments.Count < 2)
            {
                throw new ktError("Not enough arguments for ktInt::FromBase(), expected 2!", ktERR.NOTDEF);
            }

            // Get the first value and convert it to a string
            Arg = (ktValue)Arguments.First.Node.Value;
            val = Arg.ToString();
            // Get the second value and convert it to a string
            Arg = (ktValue)Arguments.First.Next.Node.Value;
            frombase = Arg.ToInt();

            // Convert 
            return _FromBase(val, frombase);
        }

        /// <summary>
        /// Convert from decimal to another base
        /// </summary>
        /// <param name="tobase">The base to convert from</param>
        /// <returns></returns>
        public ktValue _ToBase(int tobase = 2)
        {
            // Convert the value
            ktString str = Convert.ToString(m_value, tobase);
            // Create a new ktString and wrap it in a ktValue 
            return new ktValue("return", "ktString", kacTalk.Main.MakeObjectOf("ktString", str), true, true);
        }
        /// <summary>
        /// Convert from decimal to another base
        /// </summary>
        /// <param name="tobase">The base to convert from (We only use the first argument in the list)</param>
        /// <returns></returns>
        public ktValue ToBase(ktList Arguments)
        {
            ktValue Arg = ktValue.Null;
            int tobase = 0;

            // If we didn't get any arguments ...
            if (Arguments.IsEmpty())
            {
                // ... we use base 2 as a default value
                tobase = 2;
            }
            // We got arguments
            else
            {
                // Get the first value
                Arg = (ktValue)Arguments.First.Node.Value;
                // Convert the argument to an int
                tobase = Arg.ToInt();
            }

            // Convert
            return _ToBase( tobase );
        }

        /// <summary>
        /// Convert this integer to a bool
        /// </summary>
        /// <returns></returns>
        public ktValue _ToBool()
        {
            // Convert the value
            bool v = m_value != 0;
            // Create a new ktString and wrap it in a ktValue 
            return new ktValue("return", "ktBool", kacTalk.Main.MakeObjectOf("ktBool", v), true, true);
        }

        /// <summary>
        /// Assign a value to the integer
        /// </summary>
        /// <param name="Arguments">The value to assign</param>
        /// <returns>ktValue with the current object</returns>
        public ktValue Assign(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Can't assign nothing..
            if ((Arguments.IsEmpty()) || ((ktValue)Arguments.First.Node.Value).IsNull())
            {
                throw new ktError("Can't assign nothing (null) to an integer!", ktERR.NOTDEF);
            }

            // Store the first value given as argument (ignore the rest)
            m_value = GetAsInt((ktValue)Arguments.First.Node.Value);

            // Return this object wrapped in a ktValue
            return new ktValue( this.Name, "ktInt", this, m_HardType, false );
        }

        /// <summary>
        /// Get the argument/ktValue as an integer (takes in account strict typing etc)
        /// </summary>
        /// <param name="Arg">The argument</param>
        /// <returns></returns>
        protected int GetAsInt(ktValue Arg)
        {
            // Was we given a ktInt??
            if (Arg.Type == "ktInt")
            {
                // Get the value directly from the ktInt object
                return ((ktInt)Arg.Value).m_value;
            }
            // We got something else
            else
            {
                // Is this declared as a hard/strict type
                if (m_HardType)
                {
                    throw new ktError("ktInt::Assign: Cant make '" + Arg.Type + "' into a (hard) ktInt!", ktERR.WRONGTYPE);
                }
                // Not hard?
                else
                {
                    // Try to convert it to an integer (Use the ToInt of the argument value)
                    return Arg.ToInt();
                }
            }
        }

        /// <summary>
        /// Add values to the integer (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to add to the integer</param>
        /// <returns>The sum of all the arguments and the internal integer</returns>
        public ktValue _Add(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int res = 0;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't add nothing (null) to an integer!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Try to convert the argument to an int and add it to the sum
                res += GetAsInt((ktValue)L.Node.Value);
            }
            // Add the current sum to the internal integer, create a new ktInt and wrap it in a ktValue
            Value = new ktValue("return", "ktInt", new ktInt(m_value + res), true, true);

            // Return the result
            return Value;
        }
        /// <summary>
        /// Add values to the integer (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to add to the integer</param>
        /// <returns>The sum of all the arguments and the internal integer</returns>
        public ktValue Add(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't add nothing (null) to an integer!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Try to convert the argument to an int and add it to the internal integer
                m_value += GetAsInt((ktValue)L.Node.Value);
            }

            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktInt", this, m_HardType, true);

            // ... and return it
            return Value;
        }
        /// <summary>
        /// Increase the value of the object with 1
        /// </summary>
        /// <returns>This</returns>
        public ktValue Increase()
        {
            // Increase the value
            m_value++;
            // Wrap this object in a ktValue and return it
            return new ktValue("return", "ktInt", this, m_HardType, true);
        }

        /// <summary>
        /// Subtract values from the integer (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to subtract from the integer</param>
        /// <returns>The difference of all the arguments and the internal integer</returns>
        public ktValue _Subtract(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int res = m_value;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't subtract nothing (null) to an integer!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Subtract the current argument
                res -= GetAsInt((ktValue)L.Node.Value);
            }
            // Create a new ktInt and wrap it in a ktValue
            Value = new ktValue("return", "ktInt", new ktInt(res), true, true);

            return Value;
        }
        /// <summary>
        /// Subtract values from the integer (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to subtract from the integer</param>
        /// <returns>The difference of all the arguments and the internal integer</returns>
        public ktValue Subtract(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't subtract nothing (null) to an integer!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Subtract the current argument
                m_value -= GetAsInt((ktValue)L.Node.Value);
            }

            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktInt", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Decrease the value of the object with 1
        /// </summary>
        /// <returns>This</returns>
        public ktValue Decrease()
        {
            // Decrease the value
            m_value--;
            // Wrap this object in a ktValue and return i
            return new ktValue("return", "ktInt", this, m_HardType, true);
        }

        /// <summary>
        /// Multiply values with the integer (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the integer</param>
        /// <returns>The product of all the arguments and the internal integer</returns>
        public ktValue _Multiply(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int res = m_value;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't multiply nothing (null) to an integer!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Multiply with the current argument
                res *= GetAsInt((ktValue)L.Node.Value);
            }
            // Create a new ktInt and wrap it in a ktValue
            Value = new ktValue("return", "ktInt", new ktInt(res), true, true);

            return Value;
        }
        /// <summary>
        /// Multiply values with the integer (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the integer</param>
        /// <returns>The product of all the arguments and the internal integer</returns>
        public ktValue Multiply(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't multiply nothing (null) with an integer!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Multiply with the current argument
                m_value *= GetAsInt((ktValue)L.Node.Value);
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktInt", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Divide values with the integer (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to divide the integer with</param>
        /// <returns>The quotient of all the arguments and the internal integer</returns>
        public ktValue _Divide(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int res = m_value;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Check so it isn't zero
                if (a == 0)
                {
                    // Oops, can't do that!!!
                    throw new ktError("You can't divide by zero!", ktERR.DIV_BY_ZERO);
                }
                // Divide with the current argument
                res /= a;
            }
            // Create a new ktInt and wrap it in a ktValue
            Value = new ktValue("return", "ktInt", new ktInt(res), true, true);

            return Value;
        }
        /// <summary>
        /// Divide values with the integer (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to divide the integer with</param>
        /// <returns>The quotient of all the arguments and the internal integer</returns>
        public ktValue Divide(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Check so it isn't zero
                if (a == 0)
                {
                    // Oops, can't do that!!!
                    throw new ktError("You can't divide by zero!", ktERR.DIV_BY_ZERO);
                }
                // Divide with the current argument
                m_value /= a;
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktInt", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Calculate the modulus of the integer and the arguments (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the integer</param>
        /// <returns>The product of all the arguments and the internal integer</returns>
        public ktValue _Modulus(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            int res = m_value;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Do modulus with the current argument
                res %= a;
            }
            // Create a new ktInt and wrap it in a ktValue
            Value = new ktValue("return", "ktInt", new ktInt(res), true, true);

            return Value;
        }
        /// <summary>
        /// Calculate the modulus of the integer and the arguments (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the integer</param>
        /// <returns>The product of all the arguments and the internal integer</returns>
        public ktValue Modulus(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Do modulus with the current argument
                m_value %= a;
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktInt", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Calculate the value of the integer raised to the power of the arguments (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to raise the integer with</param>
        /// <returns>The value of the internal integer to the power of the arguments</returns>
        public ktValue _Power(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int res = m_value;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't raise an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Raise it to the power of the argument
                res = (int)Math.Pow((double)res,(double)a);
            }
            // Create a new ktInt and wrap it in a ktValue
            Value = new ktValue("return", "ktInt", new ktInt(res), true, true);

            return Value;
        }
        /// <summary>
        /// Calculate the value of the integer raised to the power of the arguments (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to raise the integer with</param>
        /// <returns>The value of the internal integer to the power of the arguments</returns>
        public ktValue Power(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't raise an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Raise it to the power of the argument
                m_value = (int)Math.Pow((double)m_value, (double)a);
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktInt", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Calculate the value of the integer XOR with the power of the arguments (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to XOR the integer with</param>
        /// <returns>The result</returns>
        public ktValue ExclusiveOr(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't XOR an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Perform an XOR-operation
                m_value ^= a;
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktInt", this, false, true);

            // ... and return it
            return Value;
        }
        /// <summary>
        /// Calculate the value of the integer XOR with the power of the arguments (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to XOR the integer with</param>
        public ktValue _ExclusiveOr(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            int res = m_value;
            int a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an integer with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an integer
                a = GetAsInt((ktValue)L.Node.Value);
                // Perform an XOR-operation
                res ^= a;
            }
            // Create a new ktInt and wrap it in a ktValue
            Value = new ktValue("return", "ktInt", new ktInt(res), false, true);

            return Value;
        }

        /// <summary>
        /// Compare the integer with the given value
        /// </summary>
        /// <param name="op">The operator to use for the comparison</param>
        /// <param name="val">The value to compare with</param>
        /// <returns>A value representing the comparison</returns>
        public override int Compare(ktString op, ktValue val)
        {
            int ret = 0;
            // Get the value as an integer
            int iVal = GetAsInt(val);
          
            // Check which operator we should use
            switch ( op.AsLower() )
            {
                case ">":
                case "op>":
                case "operator>":
                case "mt":
                case "gt":
                case "greater":
                case "greaterthan":
                case "more":
                case "morethan":
                case "isgreater":
                case "isgreaterthan":
                case "ismore":
                case "ismorethan":
                    {
                        ret = (m_value > iVal) ? 1 : 0;
                        break;
                    }
                case ">=":
                case "op>=":
                case "operator>=":
                case "mte":
                case "gte":
                case "greaterorequal":
                case "greaterthanorequal":
                case "moreorequal":
                case "morethanorequal":
                case "isgreaterorequal":
                case "isgreaterthanorequal":
                case "ismoreorequal":
                case "ismorethanorequal":
                    {
                        ret = (m_value >= iVal) ? 1 : 0;
                        break;
                    }
                case "<":
                case "op<":
                case "operator<":
                case "lt":
                case "less":
                case "lessthan":
                case "isless":
                case "islessthan":
                    {
                        ret = (m_value < iVal) ? 1 : 0;
                        break;
                    }
                case "<=":
                case "op<=":
                case "operator<=":
                case "lte":
                case "lessorequal":
                case "lessthanorequal":
                case "islessorequal":
                case "islessthanorequal":
                    {
                        ret = (m_value <= iVal) ? 1 : 0;
                        break;
                    }
                case "<>":
                case "!=":
                case "op<>":
                case "op!=":
                case "operator<>":
                case "operator!=":
                case "ne":
                case "isnotequal":
                case "notequal":
                    {
                        ret = (m_value != iVal) ? 1 : 0;
                        break;
                    }
                case "==":
                case "op==":
                case "operator==":
                case "isequal":
                case "equal":
                case "eq":
                    {
                        ret = (m_value == iVal) ? 1 : 0;
                        break;
                    }
                case "compare":
                    {
                        ret = m_value.CompareTo(iVal);
                        break;
                    }
                default:
                    {
                        throw new ktError("Couldn't find the method '" +
                                          op + "' in class '" + m_Name + "'.", ktERR._404);
                    }
            }

            return ret;
        }

        /// <summary>
        /// Compare the integer with the given value
        /// </summary>
        /// <param name="op">The operator to use for the comparison</param>
        /// <param name="val">The value to compare with (in a list)</param>
        /// <returns>A value representing the comparison</returns>
        public override int Compare(ktString op, ktList arguments)
        {
            // If there's exactly one argument
            if (arguments.Count == 1)
            {
                return Compare(op, (ktValue)arguments.FirstNode.Value);
            }
            // Can't compare to more than one right now!
            else
            {
                throw new ktError("Compare for more than 1 value is not implemented in '" + this.m_Name + "'!");
            }
        }

        /// <summary>
        /// Compare the integer with the given value
        /// </summary>
        /// <param name="op">The operator to use for the comparison</param>
        /// <param name="val">The value to compare with (in a list)</param>
        /// <returns>A value representing the comparison (wrapped in a ktValue)</returns>
        public ktValue _Compare(ktString Name, ktList Arguments)
        {
            // Do the comparison
            int ret = Compare(Name, Arguments);
            
            // Wrap the result in a ktValue
            return new ktValue("return", "ktInt", new ktInt(ret), false, true);
        }

        public override ktClass CreateObject(ktString Value)
        {
            return new ktInt(Value.ToInt());
        }
        public override ktClass CreateObject(object Value)
        {
            if ((Value is int) || (Value is Int32))
            {
                return new ktInt((int)Value);
            }
            else
            {
                return new ktInt((ktInt)Value);
            }
        }
        public override ktClass CreateObject(ktValue Value)
        {
            return new ktInt( GetAsInt( Value ) );
        }
        public override ktClass CreateObject()
        {
            return new ktInt();
        }
        public override string ToString()
        {
            return m_value.ToString();
        }
        public override string Export(bool AsClass)
        {
            return ToString();
        }

        protected int m_value;
    }
}
