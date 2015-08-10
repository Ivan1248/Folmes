Public Interface ICommunication
    Event NewPrivateMessage(message As Message)
    Event NewCommonMessage(message As Message)

    Event UserStatusChanged(user As User)
    Event UserInfoChanged(user As User)
End Interface
