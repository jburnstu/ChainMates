
export default { SegmentSeriesDisplay, RecentSegmentDisplay, SegmentDisplayInModal };

export function SegmentDisplayInModal(props) {
    // Display the first and last segment of the story being joined
    let firstSegment = props.storyDict.segmentHistoryList[0]
    let finalSegment = props.storyDict.segmentHistoryList.slice(-1)[0]
    firstSegment = (finalSegment == firstSegment || props.onlyShowLast) ? null : firstSegment
    const selectStory = () => props.selectStory(finalSegment.id);

    return (
        <button onClick={selectStory} className="displayStoryContainer">
            {(firstSegment == null)
                ? null
                :
                <label value="Begins:">
                    <textarea value={firstSegment.content} readOnly />
                </label>
            }
            <label value="Ends:">
                <textarea value={finalSegment.content} readOnly />
            </label>
        </button>
    )
}



export function SegmentSeriesDisplay({ segmentHistoryList, editableID, currentContent, changeSegmentSelection,handleChange }) {

    return (
        <div className="segmentSeriesContainer">
            {segmentHistoryList.map(segmentDict =>
                <SegmentDisplay key={segmentDict.id}
                    id={segmentDict.id}
                    editableID={editableID}
                    fixedContent={segmentDict.content}
                    currentContent={currentContent}
                    changeSelection={changeSegmentSelection}
                    onChange={handleChange} />
            )}
        </div>
     )
}

export function SegmentDisplay(props) {

    // Used to display segments in workshop and story-search tabs. It handles the logic
    // of whether a segment should be editable or not (based on whther it's the last
    // segment of a write-tab) which isn't the cleanest.
    // Also handles associated comments / other info being co-selected with a click on
    // the segment.

    let readOnly = true;
    let onChange = null;
    let value = props.fixedContent;
    let onClick = null;

    if (props.id == props.editableID) {
        readOnly = false;
        onChange = props.onChange;
        value = props.currentContent;

        onClick = () => {
            props.changeSelection(props.id)
        }
    }


    return (
        <textarea className={`segmentDisplay ${readOnly ? undefined : 'currentSegmentDisplay'}`} readOnly={readOnly} value={value}
            onChange={onChange} onClick={onClick} ></ textarea>)

}


export function RecentSegmentDisplay(props) {
    // Moving the display on the author-search page here for now too. Will eventually move
    // segment-selection functions here as well


    let finalSegment = props.segmentHistoryInfo.segmentHistoryList.slice(-1)[0]

    return (
        <div className="recentSegmentDisplayContainer">
            <SegmentDisplay
                id={finalSegment.id}
                editableID={null}
                fixedContent={finalSegment.content}
                currentContent={null}
                changeSelection={null}
                onChange={null} />
        </div>
    )
}