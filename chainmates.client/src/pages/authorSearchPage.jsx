import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';


import { Awards, CircleNotifications } from "../drafts/drafts";
import { PageOrTabLayout } from "../layouts/layouts";
import { RecentSegmentDisplay } from "../segmentDisplay";
import { contactAPI } from "../supportFuncs/utilityFuncs";
import { FollowButton, UnFollowButton } from "../buttons/searchButtons";
import { Activity } from "../updates/activity";
import { Notifications } from "../updates/notifications";

export default { AuthorSearchPage }
export function AuthorSearchPage(props) {

    const { authorID } = useParams();
    const [recentSegmentTraceDTOList, setRecentSegmentTraceDTOList] = useState([]);
    const [authorDict, setAuthorDict] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            if (authorID) {
                await contactAPI(`authors/${authorID}`, "get", false)
                    .then(function (value) {
                        setAuthorDict(value);
                    })
            }
            else {
                setAuthorDict(props.authorDict);
            }
        }
        fetchData();
    }, [authorID]);


    async function getRecentSegmentTraceDTOList() {
        let recentSegmentByAuthorData = await contactAPI(`authors/${authorDict.id}/recentsegments/`, "get", true, {}, []);
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


    if (!authorDict?.id) {
        return null;
    }

    return (
        <PageOrTabLayout 
            topLine={
                <AuthorHeader authorDict={authorDict} />
            }
            mainContent ={ 
                <>
                    <div className="recentSegmentsContainer">
                            <header>Recent Segments
                            </header>
                        {recentSegmentTraceDTOList.map(recentSegmentTraceDTO =>
                            <RecentSegmentDisplay key={recentSegmentTraceDTO.id} segmentTraceInfo={recentSegmentTraceDTO} />)
                        }
                    </div>
                    <div className="circleNotificationsAndAwardsContainer">
                        <CircleNotifications circleNotificationDTOList={circleNotificationDTOList} />
                        <Awards />
                    </div>
                </>
            }
            footer={
                props.self
                    ? null
                    :
                        <>
                            <FollowButton authorDict={props.authorDict} />
                            <UnFollowButton authorDict={props.authorDict} />
                        </>
            }
            rightSidebar={
                props.self
                    ? <Notifications notificationDTOList={notificationDTOList} />

                    : <Activity />
            }
        /> 
    )
}

function AuthorHeader(props) {

    console.log(props.authorDict);
    return (<div className="tabHeader authorTabHeader">
        <header>{props.authorDict.displayName}</header>
        {/*<div>{props.statsDTO.writeCount + " Segments Published"}</div>*/}
        {/*<div>{props.statsDTO.reivewCount + " Segments Reviewed"}</div>*/}
        {/*<div>{props.statsDTO.storyCount + " Stories Joined"}</div>*/}
    </div>)
}

