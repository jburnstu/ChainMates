


import React, { useEffect, useState } from "react";

import { contactAPI } from "../supportFuncs/utilityFuncs";


import { AuthorStringLink } from "../buttons/searchButtons";

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

    const parentTypeRef = {
        1: "Story",
        2: "Segment",
        3: "Comment"
    };
    console.log(dto);
    let headerContent;
    let content;
    switch (typeID) {
        // At some point will change this to a clearer enum 
        case 1:
            headerContent = "New Follow";
            content = (
                        <p>
                            <AuthorStringLink authorInfo={dto.Instigator} />
                            " started following you!";
                        </p>       
                    )
            break;
        case 2:
            headerContent = "Segment Published";
            content = (
                        <p>
                            <AuthorStringLink authorInfo={dto.Instigator} />
                            " published your segment!";
                        </p>       
                    )
            break;
        case 3:
            headerContent = "Segment Published";
            content = (
                <p>
                    <AuthorStringLink authorInfo={dto.FollowedAuthor} />
                    "'s segment was published!"
                </p>
            )
            break;
        case 4:
            content ="Someone added to " + dto.Story.Title + "!";
            // Want link to finished segment
            break;
        case 5:
            headerContent = "New Comment";
            content = (
                <p>
                    <AuthorStringLink authorInfo={dto.Instigator} />
                    " commented on your " + parentTypeRef[dto.CommentTypeId] + " ."
                </p>
            )
            break;
        case 6: //doesn't exist yet
            content = dto.Instigator.DisplayName + " liked your " + dto.targetType + " .";
            break;
        default:
            headerContent = "?";
            content = "Something happened!";
    }

    return (
        <div className="notificationContainer">
            <header>{headerContent}</header>
            {content}
        </div>
    )
}