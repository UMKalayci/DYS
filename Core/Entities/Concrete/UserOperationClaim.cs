﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Core.Entities.Abstract;

namespace Core.Entities.Concrete
{
    public class UserOperationClaim : BaseEntity, IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OperationClaimId { get; set; }

    }
}
