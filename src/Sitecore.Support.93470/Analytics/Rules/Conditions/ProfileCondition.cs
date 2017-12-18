namespace Sitecore.Support.Analytics.Rules.Conditions
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Rules;
  using Sitecore.Rules.Conditions;
  using Sitecore.Analytics;

  /// <summary>Defines the when subitem of class.</summary>
  /// <typeparam name="T">The rule context.</typeparam>
  [UsedImplicitly]
  public class ProfileCondition<T> : OperatorCondition<T> where T : RuleContext
  {
    #region Constants and Fields

    /// <summary>
    /// The profile key id;
    /// </summary>
    private string profileKeyId;

    /// <summary>
    /// The value.
    /// </summary>
    private string value;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileCondition{T}"/> class. 
    /// Initializes a new instance of the ProfileCondition class. Initializes a new instance of the ProfileCondition class.
    /// </summary>
    public ProfileCondition()
    {
      this.profileKeyId = String.Empty;
      this.value = String.Empty;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the profile key id.
    /// </summary>
    /// <value>The profile key id.</value>
    [NotNull]
    public string ProfileKeyId
    {
      get
      {
        return this.profileKeyId ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.profileKeyId = value;
      }
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    [NotNull]
    public string Value
    {
      get
      {
        return this.value ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.value = value;
      }
    }

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

      double conditionValue;
      if (!double.TryParse(this.Value, out conditionValue))
      {
        return false;
      }

      var profileKeyValue = this.GetProfileKeyValue();

      var conditionOperator = this.GetOperator();

      switch (conditionOperator)
      {
        case ConditionOperator.Equal:
          return Math.Abs(profileKeyValue - conditionValue) < 0.001;

        case ConditionOperator.GreaterThanOrEqual:
          return profileKeyValue >= conditionValue;

        case ConditionOperator.GreaterThan:
          return profileKeyValue > conditionValue;

        case ConditionOperator.LessThanOrEqual:
          return profileKeyValue <= conditionValue;

        case ConditionOperator.LessThan:
          return profileKeyValue < conditionValue;

        case ConditionOperator.NotEqual:
          return Math.Abs(profileKeyValue - conditionValue) > 0.001;
      }

      return false;
    }

    /// <summary>Gets the profile key value.</summary>
    /// <returns>Returns the int32.</returns>
    private double GetProfileKeyValue()
    {
      if (string.IsNullOrEmpty(this.ProfileKeyId))
      {
        return 0;
      }

      var item = Tracker.DefinitionDatabase.GetItem(this.ProfileKeyId);
      if (item == null)
      {
        return 0;
      }

      var parent = item.Parent;
      if (parent == null)
      {
        return 0;
      }

      var key = item.Name;

      var profile = Tracker.Current.Session.Interaction.Profiles[parent.Name];

      if (profile == null)
      {
        return 0;
      }

      return profile[key];
    }

    #endregion
  }
}