/*
 * Copyright (C) 2023 IKTSolution
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using IdeaIncubatorBlazor.Models;
using IdeaIncubatorBlazor.Services.Ideas;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace IdeaIncubatorBlazor.Views.Components;

public partial class ChatMessage : ComponentBase
{
    [Inject]
    NavigationManager? NavigationManager { get; set; }

    [Inject]
    IJSRuntime? JSRuntime { get; set; }

    [Inject]
    IChatService ChatService { get; set; }

    private HubConnection? hubConnection;

    private string username = string.Empty;

    private int chatGroupId = 0;

    public ElementReference? TextAreaRef { get; set; }

    [Parameter]
    public List<MemberInfo>? Members { get; set; }

    [Parameter]
    public Idea? SelectedIdea { get; set; }

    [Parameter]
    public int UserId { get; set; }

    [Parameter]
    public List<ChatGroupMember>? OneOnOneMembers { get; set; }

    List<Message> userMessages = new();

    List<Message>? previousMessages;

    public string? Message { get; set; }

    public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

    public bool IsOneOnOne { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsOneOnOne = false;
        username = Members.FirstOrDefault(m => m.UserId == UserId)?.UserName;
        if (OneOnOneMembers != null)
        {
            previousMessages = new List<Message>();
            chatGroupId = (int)OneOnOneMembers.FirstOrDefault().ChatGroupId;
            previousMessages = ChatService
                .GetMessagesByChatGroupId(chatGroupId)
                .OrderBy(m => m.MessageId).ToList();
            IsOneOnOne = true;
        }

        if (IsOneOnOne)
        {
            hubConnection = new HubConnectionBuilder()
           .WithUrl(NavigationManager.ToAbsoluteUri($"/chathub?username=1:1_{username}"))
           .Build();
        }
        else
        {
            hubConnection = new HubConnectionBuilder()
          .WithUrl(NavigationManager.ToAbsoluteUri($"/chathub?username={username}"))
          .Build();
        }
      

        hubConnection.On<string, string, int>("GetMessage", (user, message, groupId) =>
        {
            Message newMessage = new Message
            {
                UserName = user,
                UserId = UserId,
                MessageText = message,
                IsCurrentUser = user == username,
                DateSent = DateTime.Now,
                ChatGroupId = chatGroupId
            };
           

            if (OneOnOneMembers != null)
            {
                if(newMessage.IsCurrentUser && !newMessage.MessageText.Contains("Chat Alarm:"))
                {
                    ChatService.CreateAMessage(newMessage);   
                }

                if(chatGroupId == groupId)
                {                   
                    userMessages.Add(newMessage);
                }
            }
            else
            {
                if (groupId == 0 && !user.Contains("1:1_"))
                {
                    userMessages.Add(newMessage);
                }   
            }

            JSRuntime.InvokeVoidAsync("scrollToBottom", TextAreaRef);
            InvokeAsync(StateHasChanged);
        });


        await hubConnection.StartAsync();
    }

    private async Task Send()
    {
        if (hubConnection != null)
        {
            await hubConnection.SendAsync("AddMessageToChat", username, Message, chatGroupId);
            Message = string.Empty;
            StateHasChanged();
        }
    }

    private async Task HandleInput(KeyboardEventArgs args)
    {
        if (args.Key.Equals("Enter"))
        {
            await Send();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
