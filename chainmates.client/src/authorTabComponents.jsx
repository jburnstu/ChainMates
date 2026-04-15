import { getRandomItem, contactAPI, getArrayObjByID } from "./utilityFuncs";
import { AuthorContext } from "./context.jsx";
import React, { StrictMode, useState, authoref, useEffect, createContext, useContext } from "react";
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext, useOutlet, useNavigate } from 'react-router-dom';
export default { AuthorProfile };

export function AuthorProfile(props) {

    let writeOrReview = props.writeOrReview;
    const { tabID } = useParams();
    const [recentSegmentTraceDTOList, setRecentSegmentTraceDTOList] = useState([]);


    console.log(props.dicts, tabID)
    let authorDict = getArrayObjByID(props.dicts, tabID);
    console.log(authorDict);


    async function getRecentSegmentTraceDTOList() {
        let recentSegmentByAuthorData = await contactAPI(`authors/${authorDict.id}/recentsegments/`, "get", true, {},[]);
        console.log(recentSegmentByAuthorData);
        return recentSegmentByAuthorData;
    }

    let circleNotificationDTOList;
    let notificationDTOList;

    useEffect(() => {
        async function fetchData() {
            let segmentTraceDataArray = await getRecentSegmentTraceDTOList();
            setRecentSegmentTraceDTOList(segmentTraceDataArray);


        }
        if (authorDict?.id) {
            fetchData();
        }
    }, [authorDict?.id]);

    console.log(recentSegmentTraceDTOList)
    // const removeCurrentStory = (storyDict) => props.setDicts(storyDict, writeOrReview, "remove");

    return (
        <div className="authorTabContainer tabContainer" id={"authorTabContainer" + { tabID }}>
            <AuthorHeader statsDTO={authorDict.statsDTO} displayName={ authorDict.displayName} /> 
             <div className="authorTabContent tabContent"> 
                <div className="recentSegmentsContainer">
                    {recentSegmentTraceDTOList.map(recentSegmentTraceDTO =>
                        <RecentSegmentDisplay key={recentSegmentTraceDTO.id} segmentTraceInfo={recentSegmentTraceDTO} />)
                    }
                </div>
                <div className="circleNotificationsAndAwardsContainer">
                    <CircleNotifications circleNotificationDTOList={circleNotificationDTOList } />
                    <Awards />
                </div>
            </div>
            <div className="footer"></div>
            <Notifications notificationDTOList={notificationDTOList} />
        </div>
    )
}

function AuthorHeader(props) {

    return (<div className="tabHeader authorTabHeader">
        <div>{props.displayName}</div>
        <div>{props.statsDTO.writeCount + " Segments Published"}</div>
        <div>{props.statsDTO.reivewCount + " Segments Reviewed"}</div>
        <div>{props.statsDTO.storyCount + " Stories Joined"}</div>
    </div>)
}

function RecentSegmentDisplay(props) {

    let finalSegment = props.segmentTraceInfo.segmentHistoryList.slice(-1)[0]
    let penultimateSegment = props.segmentTraceInfo.segmentHistoryList.slice(-2)[0]

    console.log(props.segmentTraceInfo)
    console.log(finalSegment)
    return (
        <div className="recentSegmentDisplayContainer">
            <SegmentDisplay
                id={segmentDict.id}
                isFinalSegment={false}
                fixedContent={penultimateSegment.content}
                currentContent={null}
                changeSelection={null}
                onChange={null} />
            <SegmentDisplay
                id={segmentDict.id}
                isFinalSegment={false}
                fixedContent={finalSegment.content}
                currentContent={null}
                changeSelection={null}
                onChange={null} />
            )
        </div>
    )
}

function CircleNotifications(props) {

    let circleNotificationDTOList = [];

    return (
        <div className="circleNotificationListContainer">
            <header>Circle Notifications</header>
            <div>
                {circleNotificationDTOList.map((circleNotificationDTO) =>
                    <CircleNotificationPanel key={circleNotificationDTO.id}
                        circleNotificationDTO={circleNotificationDTO} />)}
            </div>
        </div>
    );
}

function CircleNotificationPanel(props) {

    let circleNotificationDTO = props.circleNotificationDTO;

    return (
        <div className="circleNotificationContainer">
            <header>{circleNotificationDTO.circleName}</header>
            <textarea readonly>{circleNotificationDTO.displayName + " added a segment to the story " + circleNotificationDTO.title + " ."}</textarea>
        </div>
    )

                }
function Awards(props) {
    //Leaving this for now!
    return (
        <div className="awardsContainer">
            <header>Awards (coming soon!)
            </header>
        </div>
    );
}

function Notifications(props) {

    let notificationDTOList = [];

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
    let content;
    switch (dto.type) {
        case "MODERATION":
            content = dto.DisplayName + " finished moderating your segment!";
            break;
        case "ADDITION":
            content = dto.DisplayName + " published a follow-up on your segment!";
            break;
        case "NEWFOLLOW":
            content = dto.DisplayName + " started following you.";
            break;
        case "PUBLISHBYFOLLOW":
            content = dto.DisplayName + "'s segment was published!";
            break;
        case "COMMENT":
            content = dto.DisplayName + " commented on your " +dto.targetType + " .";
            break;
        case "LIKE":
            content = dto.DisplayName + " liked your " + dto.targetType + " .";
            break;

    }

    return (
        <>
            <header>{dto.type}</header>
            <textarea readonly>{content}</textarea>
        </>
    )
}