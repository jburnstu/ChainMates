


import React, { useEffect, useState } from "react";

import { contactAPI } from "../supportFuncs/utilityFuncs";

export function Notifications() {
    console.log("In notifications");
    const [notificationDTOList, setNotificationDTOList] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            contactAPI("notifications/", "get", true)
                .then(function (value) {
                    setNotificationDTOList(value);
                })
        }
        console.log("in notifUseEffect");
        fetchData();
    }, [])

    console.log(notificationDTOList)
    return (
        <div className="rightSidebar notifications">
            <header>Notifications</header>
            <div className="notificationListContainer">
                {notificationDTOList.map(notificationDTO =>
                    <NotificationPanel key={notificationDTO.id}
                        notificationDTO={notificationDTO} />
                )}
            </div>
        </div>
    )
}


function NotificationPanel(props) {

    console.log(props.notificationDTO)
    let dto = props.notificationDTO.info;
    let typeID = props.notificationDTO.notificationTypeId;
    console.log(dto);
    console.log(typeID);
      

    let content;
    switch (typeID) {
        case 1:
            content = dto.Instigator.DisplayName + " started following you!";
            // Nothing else
            break;
        case 2:
            content = dto.Instigator.DisplayName + " published your segment!";
            // view segment
            break;
        case 3:
            content = dto.FollowedAuthor.DisplayName + "'s segment was published!";
            // view segment
            break;
        case 4:
            content ="Someone added to " + dto.Story.Title + "!";
            // Want link to finished segment
            break;
        case 5:
            content = dto.Instigator.DisplayName + " commented on your " + dto.ParentType + " .";
            //  View comment
            break;
        case 6: //doesn't exist yet
            content = dto.Instigator.DisplayName + " liked your " + dto.targetType + " .";
            break;
        default:
            content = "Something happened!";
    }

    return (
        <>
            <header>{dto.type}</header>
            <textarea readonly>{content}</textarea>
        </>
    )
}