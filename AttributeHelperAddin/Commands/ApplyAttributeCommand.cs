using System;
using System.ComponentModel.Design;
using AddIn.Core;
using AddIn.Core.Extensions;
using AddIn.Core.Helpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace AttributeHelperAddin.Commands
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class ApplyAttributeCommand
  {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0100;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("bea5802a-5f9a-4878-9162-4cf00cb14e1b");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly AsyncPackage package;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplyAttributeCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    /// <param name="commandService">Command service to add command to, not null.</param>
    private ApplyAttributeCommand(AsyncPackage package, OleMenuCommandService commandService)
    {
      this.package = package ?? throw new ArgumentNullException(nameof(package));
      commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

      var menuCommandID = new CommandID(CommandSet, CommandId);
      var menuItem = new MenuCommand(this.Execute, menuCommandID);
      commandService.AddCommand(menuItem);
    }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static ApplyAttributeCommand Instance
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the service provider from the owner package.
    /// </summary>
    private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
    {
      get
      {
        return this.package;
      }
    }

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static async Task InitializeAsync(AsyncPackage package)
    {
      // Switch to the main thread - the call to AddCommand in ApplyAttributeCommand's constructor requires
      // the UI thread.
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

      OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
      Instance = new ApplyAttributeCommand(package, commandService);
    }

   
    //private void Execute(object sender, EventArgs e)
    //{
    //  ThreadHelper.ThrowIfNotOnUIThread();
    //  var loaction  = WindowsFormsFileSource.DeserializeFile< CodeElementLocation>();

    //}

    private void Execute(object sender, EventArgs e)
    {
      ExecuteAsync().Forget(); // Call the async method and forget about the returned task

     
    }

    private async Task ExecuteAsync()
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      DTE2 dte = await this.package.GetDTE2Async();
      var specification = WindowsFormsFileSource.DeserializeFile<ApplyAttributeSpecification>();
     
      var codeType = dte.CodeTypeAt(specification.TargetLocation);
      codeType.SetAttributeOfMembers(specification.AttributeName, specification.SignatureArgumentPairs);
    }
  }
}
