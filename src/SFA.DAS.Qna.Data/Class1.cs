using System;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Qna.Data
{
    public class QnaDataContext : DbContext
    {
        public QnaDataContext(DbContextOptions<QnaDataContext> options) : base(options)
        {
        }
        
        
    }
}