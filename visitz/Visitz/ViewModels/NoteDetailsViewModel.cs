using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;

namespace Visitz.ViewModels
{
	public partial class NoteDetailsViewModel : VisitzViewModel
    {
        public static readonly string NoteItemKey = "noteItem";

		[ObservableProperty]
        public NoteItem noteItem;

        public override void PageCreated()
        {
            base.PageCreated();
            NoteItem = Parameters[NoteItemKey] as NoteItem;
        }
    }
}

