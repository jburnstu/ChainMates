import { getRandomItem, contactAPI, getArrayObjByID } from "./utilityFuncs";
import { AuthorContext } from "./context.jsx";
import React, { StrictMode, useState, authoref, useEffect, createContext, useContext } from "react";
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext, useOutlet, useNavigate } from 'react-router-dom';
import { SegmentDisplay } from "./storyTabComponents";

export default { AuthorProfile };

export function AuthorProfile(props) {

    const { authorID } = useParams();
    const [recentSegmentTraceDTOList, setRecentSegmentTraceDTOList] = useState([]);

    const [authorDict, setAuthorDict] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            await contactAPI(`authors/${authorID}`, "get", false)
                .then(function (value) {
                    setAuthorDict(value);
                })
        }
        if (authorID) {
            fetchData();
        }
    }, [authorID]);


    async function getRecentSegmentTraceDTOList() {
        let recentSegmentByAuthorData = await contactAPI(`authors/${authorDict.id}/recentsegments/`, "get", true, {},[]);
        console.log(recentSegmentByAuthorData);
        return recentSegmentByAuthorData;
    }

    let circleNotificationDTOList;
    let notificationDTOList;

    useEffect(() => {
        const fetchData = async () => {
            let segmentTraceDataArray = await getRecentSegmentTraceDTOList();
            setRecentSegmentTraceDTOList(segmentTraceDataArray);
        }
        if (authorDict?.id) {
            fetchData();
        }
    }, [authorDict?.id]);

    console.log(authorDict);

    return (
        <div className="authorTabContainer tabContainer" id={"authorTabContainer" + { authorID }}>
            <AuthorHeader authorDict={authorDict} /> 
             <div className="authorTabContent tabContent"> 
                <div className="recentSegmentsContainer">
                <header>Recent Segments</header>
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
            {props.self ? <Notifications notificationDTOList={notificationDTOList} /> : <Activity/>}
        </div>
    )
}

function AuthorHeader(props) {

    console.log(props.authorDict);
    return (<div className="tabHeader authorTabHeader">
        {/*<div>{props.authorDict.displayName}</div>*/}
        {/*<div>{props.statsDTO.writeCount + " Segments Published"}</div>*/}
        {/*<div>{props.statsDTO.reivewCount + " Segments Reviewed"}</div>*/}
        {/*<div>{props.statsDTO.storyCount + " Stories Joined"}</div>*/}
    </div>)
}

function RecentSegmentDisplay(props) {

    let finalSegment = props.segmentTraceInfo.segmentHistoryList.slice(-1)[0]
    let penultimateSegment = props.segmentTraceInfo.segmentHistoryList.slice(-2)[0]

    console.log(props.segmentTraceInfo)
    console.log(finalSegment)
    return (
        <div className="recentSegmentDisplayContainer">
            {/*<SegmentDisplay*/}
            {/*    id={penultimateSegment.id}*/}
            {/*    isFinalSegment={false}*/}
            {/*    fixedContent={penultimateSegment.content}*/}
            {/*    currentContent={null}*/}
            {/*    changeSelection={null}*/}
            {/*    onChange={null} />*/}
            <SegmentDisplay
                id={finalSegment.id}
                isFinalSegment={false}
                fixedContent={finalSegment.content}
                currentContent={null}
                changeSelection={null}
                onChange={null} />
        </div>
    )
}

function CircleNotifications(props) {

    let circleNotificationDTOList = [];

    //useEffect(() => {
    //    circleNotificationDTOList = await contactAPI("notifications/","get",true)
    //})

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

    const [notificationDTOList, setNotificationDTOList] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            contactAPI("notifications/", "get", true)
                .then(function (value) {
                    setNotificationDTOList(value);
                })
        }
        fetchData();
    },[])

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

function Activity() {

    <div className="rightSidebar activity">
    <header>Recent Activity</header>
    </div>
}