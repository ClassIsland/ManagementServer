using ClassIsland.Shared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassIsland.Shared.Models.Management;

/// <summary>
/// 代表一个集控认证配置。
/// </summary>
public class ManagementCredentialConfig : ObservableRecipient
{
    private string _userCredential = "";
    private string _adminCredential = "";
    private AuthorizeLevel _editAuthorizeSettingsAuthorizeLevel = AuthorizeLevel.None;
    private AuthorizeLevel _exitManagementAuthorizeLevel = AuthorizeLevel.None;
    private AuthorizeLevel _editProfileAuthorizeLevel = AuthorizeLevel.None;
    private AuthorizeLevel _editSettingsAuthorizeLevel = AuthorizeLevel.None;
    private AuthorizeLevel _exitApplicationAuthorizeLevel = AuthorizeLevel.None;
    private AuthorizeLevel _changeLessonsAuthorizeLevel = AuthorizeLevel.None;
    private AuthorizeLevel _editPolicyAuthorizeLevel = AuthorizeLevel.None;

    public string UserCredential
    {
        get => _userCredential;
        set => SetProperty(ref _userCredential, value);
    }

    public string AdminCredential
    {
        get => _adminCredential;
        set => SetProperty(ref _adminCredential, value);
    }

    public AuthorizeLevel EditAuthorizeSettingsAuthorizeLevel
    {
        get => _editAuthorizeSettingsAuthorizeLevel;
        set => SetProperty(ref _editAuthorizeSettingsAuthorizeLevel, value);
    }

    public AuthorizeLevel EditPolicyAuthorizeLevel
    {
        get => _editPolicyAuthorizeLevel;
        set => SetProperty(ref _editPolicyAuthorizeLevel, value);
    }

    public AuthorizeLevel ExitManagementAuthorizeLevel
    {
        get => _exitManagementAuthorizeLevel;
        set => SetProperty(ref _exitManagementAuthorizeLevel, value);
    }

    public AuthorizeLevel EditProfileAuthorizeLevel
    {
        get => _editProfileAuthorizeLevel;
        set => SetProperty(ref _editProfileAuthorizeLevel, value);
    }

    public AuthorizeLevel EditSettingsAuthorizeLevel
    {
        get => _editSettingsAuthorizeLevel;
        set => SetProperty(ref _editSettingsAuthorizeLevel, value);
    }

    public AuthorizeLevel ExitApplicationAuthorizeLevel
    {
        get => _exitApplicationAuthorizeLevel;
        set => SetProperty(ref _exitApplicationAuthorizeLevel, value);
    }

    public AuthorizeLevel ChangeLessonsAuthorizeLevel
    {
        get => _changeLessonsAuthorizeLevel;
        set => SetProperty(ref _changeLessonsAuthorizeLevel, value);
    }
}
