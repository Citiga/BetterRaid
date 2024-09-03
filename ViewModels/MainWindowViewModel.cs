﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Avalonia.Controls;
using BetterRaid.Extensions;
using BetterRaid.Misc;
using BetterRaid.Models;
using BetterRaid.Services;
using BetterRaid.Views;

namespace BetterRaid.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private string? _filter;
    private ObservableCollection<TwitchChannel> _channels = [];
    private BetterRaidDatabase? _db;

    public BetterRaidDatabase? Database
    {
        get => _db;
        set
        {
            if (SetProperty(ref _db, value) && _db != null)
            {
                LoadChannelsFromDb();
            }
        }
    }

    public ObservableCollection<TwitchChannel> Channels
    {
        get => _channels;
        set => SetProperty(ref _channels, value);
    }

    public string? Filter
    {
        get => _filter;
        set => SetProperty(ref _filter, value);
    }

    public bool IsLoggedIn => App.TwitchApi != null;

    public MainWindowViewModel(ITwitchDataService t)
    {
        Console.WriteLine(t);
        Console.WriteLine("[DEBUG] MainWindowViewModel created");
    }
    
    public void ExitApplication()
    {
        //TODO polish later
        Environment.Exit(0);
    }

    public void ShowAboutWindow(Window owner)
    {
        var about = new AboutWindow();
        about.InjectDataContext<AboutWindowViewModel>();
        about.ShowDialog(owner);
        about.CenterToOwner();
    }

    public void LoginWithTwitch()
    {
        Tools.StartOAuthLogin(App.TwitchOAuthUrl, OnTwitchLoginCallback, CancellationToken.None);
    }

    private void OnTwitchLoginCallback()
    {
        OnPropertyChanged(nameof(IsLoggedIn));
    }

    private void LoadChannelsFromDb()
    {
        if (_db == null)
        {
            return;
        }
        
        Channels.Clear();

        var channels = _db.Channels
            .Select(channelName => new TwitchChannel(channelName))
            .ToList();
        
        foreach (var c in channels)
        {
            Channels.Add(c);
        }
    }
}
