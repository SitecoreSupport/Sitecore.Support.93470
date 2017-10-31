namespace Sitecore.Support.Analytics.Rules.Conditions
{
  using Sitecore.Analytics;
  using Sitecore.Diagnostics;
  using Sitecore.Rules;
  using Sitecore.Rules.Conditions;

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
      Assert.IsNotNull(Tracker.Current, "Tracker.Current is not initialized");
      Assert.IsNotNull(Tracker.Current.Session, "Tracker.Current.Session is not initialized");
      Assert.IsNotNull(Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");

      var value = Tracker.Current.Session.Interaction.Value;

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