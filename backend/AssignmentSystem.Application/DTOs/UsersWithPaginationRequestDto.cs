using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSystem.Application.DTOs
{
    public class UsersWithPaginationRequestDto
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
