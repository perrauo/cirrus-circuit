using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cirrus.FSM
{
    // TODO : use IState everywhere?

    public interface IState
    {
        //public Resource resource;        //public Resource resource;
        //public object[] context;

        int Id { get; }// { return resource.Id; } }

        void Enter(params object[] args);
        void Exit();
        void Reenter(params object[] args);

        void BeginTick();
        void EndTick();
        void OnDrawGizmos();


        //Transition Transitions { get; }

    }
}

