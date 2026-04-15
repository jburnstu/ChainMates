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
    console.log(AuthorDict);


    async function getRecentSegmentTraceDTOList() {
        let segmentByAuthorData = await contactAPI(`authors/${authorDict.id}/recentsegments`, "get");
        console.log(segmentByAuthorData);
        return segmentByAuthorData;
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
        <div className="authorProfileContainer" id={"authorProfileContainer" + { tabID }}>
            <AuthorHeader statsDTO={authorDict.statsDTO} displayName={ authorDict.displayName} /> 
             <div className = "authorInfoContainer"> 
                <div className="recentSegmentsContainer">
                    {recentSegmentTraceDTOList.map(recentSegmentTraceDTO =>
                        <RecentSegmentDisplay key={recentSegmentTraceDTO.id} segmentTraceInfo={recentSegmentTraceDTO} />)
                    }
                </div>
                <div className="lowerSection">
                    <CircleNotifications circleNotificationDTOList={circleNotificationDTOList } />
                    <Awards />
                </div>
            </div>
            <Notifications notificationDTOList={notificationDTOList} className="rightSidebar comments"/>
        </div>
    )
}

function RecentSegmentDisplay(props) {

    let finalSegment = props.segmentTraceInfo.segmentHistoryList.slice(-1)[0]
    let penultimateSegment = props.segmentTraceInfo.segmentHistoryList.slice(-2)[0]

    console.log(props.segmentTraceInfo)
    console.log(finalSegment)
    return (
        <div>
            <textarea value={penultimateSegment.content}></textarea>
            <textarea value={finalSegment.content}></textarea>
        </div>
    )
}

function AuthorHeader(props) {


    return (<div className="storyHeader">
        <div>{props.displayName}</div>
        <div>{props.statsDTO.writeCount + " Segments Published"}</div>
        <div>{props.statsDTO.reivewCount + " Segments Reviewed"}</div>
        <div>{props.statsDTO.storyCount + " Stories Joined"}</div>
    </div>)
}

function CircleNotifications(props) {

    let circleNotificationDTOList;

    return (
        <>
            <header>Circle Notifications</header>
            <div className="circleNotificationContainer">
                {circleNotificationDTOList.map((circleNotificationDTO) =>
                    <CircleNotificationPanel key={circleNotificationDTO.id}
                        circleNotificationDTO={circleNotificationDTO} />)})
            </div>
        </>
    );
}

function CircleNotificationPanel(props) {

    let circleNotificationDTO = props.circleNotificationDTO;

    return (
        <>
            <header>{circleNotificationDTO.circleName}</header>
            <textarea readonly>{circleNotificationDTO.displayName + " added a segment to the story " + circleNotificationDTO.title + " ."}</textarea>
        </>
    )

                }
function Awards(props) {
    //Leaving this for now!
    retun(<div>Coming Soon!</div>);
}

function Notifications(props) {

   let notificationDTOList;

    return (
        <>
            <header>Comments</header>
            <div className="notificationListContainer">
                {notificationDTOList.map(notificationDTO =>
                    <NotificationPanel key={notificationDTO.id}
                        notificationDTO={notificationDTO} />
                    )}
            </div>
        </>
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