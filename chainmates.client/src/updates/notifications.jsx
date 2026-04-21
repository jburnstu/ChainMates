


import React, { useEffect, useState } from "react";

import { contactAPI } from "../supportFuncs/utilityFuncs";

export function Notifications() {

    /////// Grab notifications when the component is loaded --  ///////
    /////// not passed in initial data load  //////////////////////////
    const [notificationDTOList, setNotificationDTOList] = useState([]);
    useEffect(() => {
        const fetchData = async () => {
            contactAPI("notifications/", "get", true)
                .then(function (value) {
                    setNotificationDTOList(value);
                })
        }
        fetchData();
    }, [])

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

    let dto = props.notificationDTO.info;
    let typeID = props.notificationDTO.notificationTypeId;
      
    let content;
    switch (typeID) {
        // At some point will change this to a clearer enum 
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