


import React, { StrictMode, useState, useEffect } from "react";

import { contactAPI } from "./utilityFuncs.jsx";


export function Notifications() {

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

    let dto = props.notificationDTO;
    console.log(dto);
    let typeID = dto.notificationTypeId;

    let content;
    switch (typeID) {
        case "authorFollowedYou":
            content = dto.DisplayName + " started following you.";
            // Nothing else
            break;
        case "authorApprovedYourSegment":
            content = "Your segment was published!";
            // view segment
            break;
        case "authorApprovedSegmentYouFollow":
            content = dto.DisplayName + "'s segment was published!";
            // view segment
            break;
        case "authorApprovedSegmentInYourChain":
            content = dto.DisplayName + " published a follow-up on your segment!";
            // Want link to finished segment
            break;
        case "authorAddedComment":
            content = dto.DisplayName + " commented on your " + dto.ParentType + " .";
            //  View comment
            break;
        case "LIKE": //doesn't exist yet
            content = dto.DisplayName + " liked your " + dto.targetType + " .";
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