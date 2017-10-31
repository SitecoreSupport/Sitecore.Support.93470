// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasGoalCondition.cs" company="Sitecore A/S">
//   Copyright (C) 2012 by Sitecore A/S
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Analytics.Rules.Conditions
{
  using System;
  using System.Linq; 
  using Sitecore.Diagnostics;
  using Sitecore.Rules;
  using Sitecore.Rules.Conditions;

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
      Assert.IsNotNull(Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");

      try
      {
        this.GoalGuid = new Guid(this.GoalId);
      }
      catch
      {
        Log.Warn(string.Format("Could not convert value to guid: {0}", this.GoalId), this.GetType());
        return false;
      }

      return Tracker.Current.Session.Interaction.Pages.Any(page => page.PageEvents.Any(e => e.PageEventDefinitionId == this.GoalGuid));
    }

    #endregion
  }
}