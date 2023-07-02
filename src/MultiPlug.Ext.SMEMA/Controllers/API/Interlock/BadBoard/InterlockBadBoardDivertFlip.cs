﻿using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock.BadBoard
{
    [Route("1/lane/interlock/bad/divert/flip")]
    public class InterlockBadBoardDivertFlip : APIController
    {
        public Response Post(string index, string guid, bool enable)
        {
            LaneComponent LaneSearch = GetLane.Invoke(index, guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            if (LaneSearch.Interlock.PermissionInterfaceREST == true)
            {
                LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivertFlip(enable);
            }

            var Result = new
            {
                Bad = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoard,
                Latched = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardLatch,
                Diverted = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivert,
                DivertedLatched = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivertLatch,
                Flip = LaneSearch.Interlock.BoardAvailableStateMachine.FlipBoard,
                FlipLatched = LaneSearch.Interlock.BoardAvailableStateMachine.FlipBoardLatch
            };

            return new Response
            {
                StatusCode = LaneSearch.Interlock.PermissionInterfaceREST == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                Model = Result,
                MediaType = "application/json"
            };
        }
    }
}
