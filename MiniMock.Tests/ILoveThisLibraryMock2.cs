namespace MiniMock.Tests;

/// <summary>
/// Mock implementation of <see cref="MiniMock.Tests.ILoveThisLibrary"/>. Should only be used for testing purposes.
/// </summary>
internal class ILoveThisLibraryMock2 : MiniMock.Tests.ILoveThisLibrary
{
    private ILoveThisLibraryMock2(System.Action<Config>? config = null)
    {
        var result = new Config(this);
        config = config ?? new System.Action<Config>(t => { });
        config.Invoke(result);
        this._MockConfig = result;
    }

    public static MiniMock.Tests.ILoveThisLibrary Create(System.Action<Config>? config = null) => new ILoveThisLibraryMock2(config);

    internal Config _MockConfig { get; set; }

    public partial class Config
    {
        private readonly ILoveThisLibraryMock2 target;

        public Config(ILoveThisLibraryMock2 target)
        {
            this.target = target;
        }
    }

    #region Property : CurrentVersion
    System.Version MiniMock.Tests.ILoveThisLibrary.CurrentVersion
    {
        get => this.Get_CurrentVersion_15();
        set => this.Set_CurrentVersion_15(value);
    }

    private System.Version? internal_CurrentVersion_15;
    internal System.Func<System.Version> Get_CurrentVersion_15 { get; set; } = () => throw new System.InvalidOperationException("The property 'CurrentVersion' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.Tests.ILoveThisLibrary.CurrentVersion" };
    internal System.Action<System.Version> Set_CurrentVersion_15 { get; set; } = s => throw new System.InvalidOperationException("The property 'CurrentVersion' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.Tests.ILoveThisLibrary.CurrentVersion" };

    #endregion
    public partial class Config
    {

        /// <summary>
        ///     Sets a initial value for CurrentVersion.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config CurrentVersion(System.Version value)
        {
            this.target.internal_CurrentVersion_15 = value;
            this.target.Get_CurrentVersion_15 = () => this.target.internal_CurrentVersion_15;
            this.target.Set_CurrentVersion_15 = s => this.target.internal_CurrentVersion_15 = s;
            return this;
        }


        /// <summary>
        ///     Specifies a getter and setter method to call when the property CurrentVersion is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config CurrentVersion(System.Func<System.Version> get, System.Action<System.Version> set)
        {
            this.target.Get_CurrentVersion_15 = get;
            this.target.Set_CurrentVersion_15 = set;
            return this;
        }

    }

    #region Method : bool DownloadExists(string version)
    public delegate bool On_DownloadExists_121_Delegate(string version);

    bool MiniMock.Tests.ILoveThisLibrary.DownloadExists(string version)
    {
        return this.On_DownloadExists_121.Invoke(version);
    }
    internal On_DownloadExists_121_Delegate On_DownloadExists_121 { get; set; } = (string version) => throw new System.InvalidOperationException("The method 'DownloadExists' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.Tests.ILoveThisLibrary.DownloadExists(string)" };

    public partial class Config
    {
        private Config _On_DownloadExists_121(On_DownloadExists_121_Delegate call)
        {
            this.target.On_DownloadExists_121 = call;
            return this;
        }
    }

    #endregion

    public partial class Config
    {

        /// <summary>
        ///     Configures the mock to execute the specified action when the method matching the signature is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadExists(On_DownloadExists_121_Delegate call)
        {
            this._On_DownloadExists_121(call);
            return this;
        }


        /// <summary>
        ///     Configures the mock to throw the specified exception when the method is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadExists(System.Exception throws)
        {
            this._On_DownloadExists_121((string version) => throw throws);
            return this;
        }


        /// <summary>
        ///     Configures the mock to return the specific value when returning <see cref="bool"/>
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadExists(bool returns)
        {
            this._On_DownloadExists_121((string version) => returns);
            return this;
        }

    }

    #region Method : System.Threading.Tasks.Task<System.Uri> DownloadLinkAsync(string version)
    public delegate System.Threading.Tasks.Task<System.Uri> On_DownloadLinkAsync_122_Delegate(string version);

    System.Threading.Tasks.Task<System.Uri> MiniMock.Tests.ILoveThisLibrary.DownloadLinkAsync(string version)
    {
        return this.On_DownloadLinkAsync_122.Invoke(version);
    }
    internal On_DownloadLinkAsync_122_Delegate On_DownloadLinkAsync_122 { get; set; } = (string version) => throw new System.InvalidOperationException("The method 'DownloadLinkAsync' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.Tests.ILoveThisLibrary.DownloadLinkAsync(string)" };

    public partial class Config
    {
        private Config _On_DownloadLinkAsync_122(On_DownloadLinkAsync_122_Delegate call)
        {
            this.target.On_DownloadLinkAsync_122 = call;
            return this;
        }
    }

    #endregion

    public partial class Config
    {

        /// <summary>
        ///     Configures the mock to execute the specified action when the method matching the signature is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadLinkAsync(On_DownloadLinkAsync_122_Delegate call)
        {
            this._On_DownloadLinkAsync_122(call);
            return this;
        }


        /// <summary>
        ///     Configures the mock to throw the specified exception when the method is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadLinkAsync(System.Exception throws)
        {
            this._On_DownloadLinkAsync_122((string version) => throw throws);
            return this;
        }


        /// <summary>
        ///     Configures the mock to return the specific value when returning <see cref="System.Threading.Tasks.Task&lt;System.Uri&gt;"/>
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadLinkAsync(System.Threading.Tasks.Task<System.Uri> returns)
        {
            this._On_DownloadLinkAsync_122((string version) => returns);
            return this;
        }


        /// <summary>
        ///     Configures the mock to return the specific value when returning a generic task containing <see cref="System.Threading.Tasks.Task&lt;System.Uri&gt;"/>
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadLinkAsync(System.Uri returns)
        {
            this._On_DownloadLinkAsync_122((string version) => System.Threading.Tasks.Task.FromResult(returns));
            return this;
        }


        /// <summary>
        ///     Configures the mock to call the specified function and return the value wrapped in a task object when the method matching the signature is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config DownloadLinkAsync(System.Func<string, System.Uri> call)
        {
            this._On_DownloadLinkAsync_122((version) => System.Threading.Tasks.Task.FromResult(call(version)));
            return this;
        }

    }
    #region Indexer : System.Version this[string]
    System.Version MiniMock.Tests.ILoveThisLibrary.this[string index]
    {
        get => this.On_IndexGet_3(index);
        set => this.On_IndexSet_3(index, value);
    }

    internal System.Func<string, System.Version> On_IndexGet_3 { get; set; } = (_) => throw new System.InvalidOperationException("The indexer 'this[]' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.Tests.ILoveThisLibrary.this[string]" };
    internal System.Action<string, System.Version> On_IndexSet_3 { get; set; } = (_, _) => throw new System.InvalidOperationException("The indexer 'this[]' in 'ILoveThisLibrary' is not explicitly mocked.") { Source = "MiniMock.Tests.ILoveThisLibrary.this[string]" };

    #endregion
    public partial class Config
    {

        /// <summary>
        ///     Gets and sets values in the dictionary when the indexer is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config Indexer(System.Collections.Generic.Dictionary<string, System.Version> values)
        {
            this.target.On_IndexGet_3 = s => values[s];
            this.target.On_IndexSet_3 = (s, v) => values[s] = v;
            return this;
        }


        /// <summary>
        ///     Specifies a getter and setter method to call when the indexer for <see cref="string"/> is called.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config Indexer(System.Func<string, System.Version> get, System.Action<string, System.Version> set)
        {
            this.target.On_IndexGet_3 = get;
            this.target.On_IndexSet_3 = set;
            return this;
        }

    }

    #region System.EventHandler<System.Version> NewVersionAdded
    internal event System.EventHandler<System.Version>? NewVersionAdded_7;
    event System.EventHandler<System.Version>? MiniMock.Tests.ILoveThisLibrary.NewVersionAdded
    {
        add => this.NewVersionAdded_7 += value;
        remove => this.NewVersionAdded_7 -= value;
    }
    internal void trigger_NewVersionAdded_7(object? sender, System.Version e)
    {
        this.NewVersionAdded_7?.Invoke(sender, e);
    }

    #endregion
    public partial class Config
    {

        /// <summary>
        ///     Returns a action that can be used for triggering NewVersionAdded.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config NewVersionAdded(out System.Action<System.Version> trigger)
        {
            trigger = args => this.NewVersionAdded(this.target, args);
            return this;
        }


        /// <summary>
        ///     Trigger NewVersionAdded directly.
        /// </summary>
        /// <returns>The updated configuration.</returns>
        public Config NewVersionAdded(object? sender, System.Version e)
        {
            this.target.trigger_NewVersionAdded_7(sender, e);
            return this;
        }

    }
}
