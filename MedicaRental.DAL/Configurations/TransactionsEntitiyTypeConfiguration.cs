using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations
{
    public class TransactionItemEntitiyTypeConfiguration : IEntityTypeConfiguration<TransactionItem>
    {
        public void Configure(EntityTypeBuilder<TransactionItem> builder)
        {
            builder.HasOne(r => r.Transaction)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
            /*Severity	Code	Description	Project	File	Line	Suppression State
            Error	CS0308	The non-generic method 'ReferenceCollectionBuilder<Transaction, 
            TransactionItem>.HasForeignKey(params string[])' 
            cannot be used with type arguments	MedicaRental.DAL	
           	18	Active
            */
            /*Introducing FOREIGN KEY constraint 'FK_TransactionItems_Transactions_TransactionId'
             * on table 'TransactionItems' may cause cycles or multiple cascade paths. 
             * Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY 
             * constraints.
            Could not create constraint or index. See previous errors.
            */
        }
    }
}
