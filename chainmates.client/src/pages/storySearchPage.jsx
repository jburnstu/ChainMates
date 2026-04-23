import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';

import { PageOrTabLayout } from "../layouts/layouts";
import { contactAPI } from "../supportFuncs/utilityFuncs";

export default { StorySearchPage, StorySubSearchPage };


async function changeFinalSegment(finalSegmentID) {
    await contactAPI(`segments/${finalSegmentID}/history`, "get")
        .then(function (value) {
            setFinalSegmentDTO(value);
        })
}

export function StorySearchPage() {

    const { storyID } = useParams();
    const [storyInfo, setStoryInfo] = useState(null);
    const [finalSegmentDTO, setFinalSegmentDTO] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            await contactAPI(`stories/${storyID}`, "get", false)
                .then(function (value) {
                    setStoryInfo(value);
                    console.log(value);
                    changeFinalSegment(value.structure[0][0]); // The first segment in the story is saved at 0 (in an array of one)
                    //navigate(`/stories/${storyID}/${firstSegmentID}/`);
                });
        }
        if (storyID) {
            fetchData();
        }
    }, [storyID]);



    ///////////////// Assign each segment an on-off selection for the Comments + Info sidebar ///////////////
    let noSelections = {};
    finalSegmentDTO.forEach(dictInArray =>
        noSelections[dictInArray.id] = false);
    const [selectedSegmentDict, setSelectedSegmentDict] = useState(noSelections);
    function changeSegmentSelection(segmentID) {
        setSelectedSegmentDict({ ...selectedSegmentDict, [segmentID]: !selectedSegmentDict[segmentID] })
    }

    if (!storyInfo) {
        if (storyInfo != null) {
            console.log("DIDN@T WORK");
        }
        return null;
    }

    return (
        <PageOrTabLayout 
            topLine={
                <StorySeachPageTopLine storyInfo={storyInfo} />
            }
            mainContent={
                <SegmentSeriesDisplay segmentHistoryList={finalSegmentDTO.segmentHistoryList}
                    editableID={null}
                    currentContent={null}
                    changeSelection={changeSegmentSelection}
                    onChange={null}
                /> 
            }
            footer={
                <>
                    <GoUpASegmentButton previousSegmentID={} />
                    <GoDownASegmentButton structureDict={structure} />
                </>
            }
            rightSidebar={
                <Comments selections={selectedSegmentDict} storyDict={storyInfo} />
            
            }
        /> 
    )
}

function StorySeachPageTopLine() { }

function GoDownASegmentButton({ structureDict }) {
    const [isOpen, setIsOpen] = useState(false);
    function createModal() {
        setIsOpen(true);
        console.log("modal clicked")
    }

    return (
        <>
            <button onClick={createModal}> Start A New Story
            </ button >
            <ModalWindow isOpen={isOpen} onClose={() => setIsOpen(false)}>
                <div className="allDisplayStoriesContainer">
                    <NewSegmentOptionspanel   close={() => setIsOpen(false)} />
                </div>
            </ModalWindow >
        </>
    )

}

function GoUpASegmentButton({previousSegmentID}) {

    return (
        <button onClick={() => changeFinalSegment(penultimateSegmentID)}/>
    )
}




//export function StorySubSearchPage() {
//    const { finalSegmentID } = useParams();

//    return (
//        <SegmentSeriesDisplay storyDict={storyDict}
//            editableID={null}
//            currentContent={null}
//            changeSelection={changeSegmentSelection}
//            onChange={null}
//        />
//    )
//}