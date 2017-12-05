// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasGoalCondition.cs" company="Sitecore A/S">
//   Copyright (C) 2012 by Sitecore A/S
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Support.Analytics.Rules.Conditions
{
  using System;
  using System.Linq;
  using Sitecore.Analytics;
  using Sitecore.Analytics.Core;
  using Sitecore.Analytics.Data;
  using Sitecore.Analytics.Model;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Diagnostics;
  using Sitecore.Rules.Conditions;
  using Sitecore.Rules;
  using System.Collections.Generic;

  /// <summary>Defines the when subitem of class.</summary>
  /// <typeparam name="T">The rule context.</typeparam>
  public class HasGoalCondition<T> : WhenCondition<T> where T : RuleContext
  {
    #region Properties

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    [NotNull]
    public string GoalId { get; set; }

    #endregion

    /// <summary>
    /// Gets or sets the goal GUID.
    /// </summary>
    /// <value>The goal GUID.</value>
    private Guid GoalGuid { get; set; }


    #region Methods

    /// <summary>Executes the specified rule context.</summary>
    /// <param name="ruleContext">The rule context.</param>
    /// <returns><c>True</c>, if the condition succeeds, otherwise <c>false</c>.</returns>
    protected override bool Execute([NotNull] T ruleContext)  
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      Assert.IsNotNull(Tracker.Current, "Tracker.Current is not initialized");
      Assert.IsNotNull(Tracker.Current.Session, "Tracker.Current.Session is not initialized");
      try
      {
        this.GoalGuid = new Guid(this.GoalId);
      }
      catch
      {
        Log.Warn(string.Format("Could not convert value to guid: {0}", (object)this.GoalId), (object)this.GetType());
        return false;
      }
      if (Tracker.Current.Session.Interaction != null && (Tracker.Current.Session.Interaction.Pages.Count() > 1 || Tracker.Current.Session.Interaction.CurrentPage.Url.Path != null))
        return ((IEnumerable<Page>)Tracker.Current.Session.Interaction.Pages).Any<Page>((Func<Page, bool>)(page => page.PageEvents.Any<Sitecore.Analytics.Model.PageEventData>((Func<Sitecore.Analytics.Model.PageEventData, bool>)(e => e.PageEventDefinitionId == this.GoalGuid))));
      var history = Tracker.Current.Contact.LoadHistorycalData(1);
      if (history == null || history.Count<IInteractionData>() == 0) 
      {
        return false;
      }
      IInteractionData interactionData = history.First<IInteractionData>();
      if (interactionData != null)
        return ((IEnumerable<Page>)interactionData.Pages).SelectMany<Page, Sitecore.Analytics.Model.PageEventData>((Func<Page, IEnumerable<Sitecore.Analytics.Model.PageEventData>>)(page => page.PageEvents)).Any<Sitecore.Analytics.Model.PageEventData>((Func<Sitecore.Analytics.Model.PageEventData, bool>)(pageEvent =>
        {
          if (pageEvent.IsGoal)
            return pageEvent.PageEventDefinitionId == this.GoalGuid;
          return false;
        }));
      return false;

    }

    #endregion
  }
}