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

    /////////////// Load up the author's data, return null if it  /////////
    //////////////  can't be found (/while it's being loaded)    /////////////////////
    const { authorID } = useParams();
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

    ///// Load up some of the author's most recent segments 

    const [recentSegmentHistoryDTOList, setRecentSegmentHistoryDTOList] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            let segmentHistoryDTOList = await contactAPI(`authors/${authorDict.id}/recentsegments/`, "get", true, {}, []);
            setRecentSegmentHistoryDTOList(segmentHistoryDTOList);
        }
        if (authorDict?.id) {
            fetchData();
        }
    }, [authorDict?.id]);


    // Circles aren't up and running yet
    let circleNotificationDTOList;

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
                        <header>Recent Segments</header>
                         <div className="recentSegmentsArray">
                        {recentSegmentHistoryDTOList.map(recentSegmentHistoryDTO =>
                            <RecentSegmentDisplay key={recentSegmentHistoryDTO.id} segmentHistoryInfo={recentSegmentHistoryDTO} />)
                            }
                        </div>
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
                    // NB: haven't yet set up these buttons to refuse invalid calls, or to not 
                    // be present on the author's own page if searched
                        <>
                            <FollowButton authorDict={authorDict} />
                            <UnFollowButton authorDict={authorDict} />
                        </>
            }
            rightSidebar={
                props.self
                    ? <Notifications/>
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

