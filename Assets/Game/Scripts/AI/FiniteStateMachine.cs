using System.Collections;
using System.Collections.Generic;
using My.Utils;
using UnityEngine;

namespace FSM
{
    public class FiniteStateMachine : MonoBehaviour
    {
        public float m_alertTimer;
        float m_inspectTimer, m_deltaTime;

       public struct StatesFSM
       {
           public int test;
       }
        public bool m_emitSoundAtRandomPos, m_isSoundEmitted, m_isSoundFromOldSpot = false;

        public bool m_isIdle        {get; private set;} = true;
        public bool m_isAlerted     {get; private set;} = false;
        public bool m_isInspecting  {get; private set;} = false;
        public bool m_isDead        {get; private set;} = false;
        public bool m_isProvoked    {get; private set;} = false;

        void Update()
        {
            CacheVars();

            // States
            AlertStateBehavior();
            InspectStateBehavior();

            m_alertTimer -= m_alertTimer > 0 && !m_isInspecting ? m_deltaTime : 0;
            m_inspectTimer -= m_inspectTimer > 0 && !m_isSoundFromOldSpot ? m_deltaTime : 0;

            // print(m_States.Peek());
        }

        void CacheVars()
        {
            m_deltaTime = Time.deltaTime;
        }

        void AlertStateBehavior()
        {
            if ( m_isSoundEmitted && m_alertTimer > 0 )
            {
                // isSoundEmitted = false;
                m_isIdle = false;
                m_isAlerted = true;
            }
            else
            {
                // isAlerted = false;
                m_isIdle = true;
            }
        }

        private void InspectStateBehavior()
        {
            if (m_isAlerted && m_isSoundFromOldSpot)
            {
                m_isInspecting = true;
            }
            else
            {
                m_isInspecting = false;
                m_isSoundFromOldSpot = false;
                m_isSoundEmitted = false;
            }
        }

        public void Set_isIdle(bool b) { m_isIdle = b; }
        public void Set_isAlerted(bool b) { m_isAlerted = b; }
        public void Set_isInspecting(bool b) { m_isInspecting = b; }
        public void Set_isDead(bool b) { m_isDead = b; }
        public void Set_isProvoked(bool b) { m_isProvoked = b; }
    }
}
