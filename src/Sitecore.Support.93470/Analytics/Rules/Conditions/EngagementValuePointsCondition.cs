namespace Sitecore.Support.Analytics.Rules.Conditions
{
  using System;
  using System.Linq;
  using Sitecore.Analytics;
  using Sitecore.Analytics.Data;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Diagnostics;
  using Sitecore.Rules;
  using Sitecore.Rules.Conditions;
  using System.Collections.Generic;


  /// <summary>Defines the when template is class.</summary>
  /// <typeparam name="T">The rule context.</typeparam>
  [UsedImplicitly]
  public class EngagementValuePointsCondition<T> : OperatorCondition<T> where T : RuleContext
  {
    #region Properties

    /// <summary>
    /// Gets or sets the template id.
    /// </summary>
    /// <value>The template id.</value>
    public int Value { get; set; }

    #endregion

    #region Methods

    /// <summary>Executes the specified rule context.</summary>
    /// <param name="ruleContext">The rule context.</param>
    /// <returns><c>True</c>, if the condition succeeds, otherwise <c>false</c>.</returns>
    protected override bool Execute([NotNull] T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      var interaction = Tracker.Current.Session.Interaction;
      int value;
      if (interaction != null)
      {
        value = interaction.Value;
      }
      else
      {
        var history = Tracker.Current.Contact.LoadHistorycalData(1);
        if (history == null || history.Count<IInteractionData>() == 0)
        {
          return false;
        }
        IInteractionData interactionData = history.First<IInteractionData>();
        if (interactionData == null)
        {
          return false;          
        }
        value = interactionData.Value;
      }
      var conditionOperator = this.GetOperator();

      switch (conditionOperator)
      {
        case ConditionOperator.Equal:
          return value == this.Value;

        case ConditionOperator.GreaterThanOrEqual:
          return value >= this.Value;

        case ConditionOperator.GreaterThan:
          return value > this.Value;

        case ConditionOperator.LessThanOrEqual:
          return value <= this.Value;

        case ConditionOperator.LessThan:
          return value < this.Value;

        case ConditionOperator.NotEqual:
          return value != this.Value;
      }

      return false;
    }

    #endregion
  }
}