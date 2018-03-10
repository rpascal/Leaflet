
/*** creates base geojson layer ***/
//function createGeoJsonLayer(model) {
//    var baseModel = {
//        data: {},
//        baseStyle: function (feature) { return { color: feature.properties.PolyBorderColor } },
//        newStyles: function (feature) { return {} },
//        onEachFeature: function (feature, layer) { }
//    }

//    Object.assign(baseModel, model);
//    var layer = L.geoJson(baseModel.data, {
//        pane: 'radarPane',
//        style: function (feature) {
//            return Object.assign(baseModel.baseStyle(feature), baseModel.newStyles(feature));
//        },
//        onEachFeature: baseModel.onEachFeature,
//    });

//    return layer;
//}

function createImageLayer(imageUrl, imageBounds) {


    var layer = L.imageOverlay(imageUrl, imageBounds, { pane: 'radarPane' });
    //layer.setZIndex(10000);

    return layer;
}



/**********************************************************
        Extending the leaflet library - Start
    **********************************************************/

L.Layer.include({
    leafAddToMap: function () {
       return this.addTo(getMap());
    }
});

L.LayerGroup.include({
    leafGetLayer: function (id, property) {
        property = property || "id";
        for (var i in this._layers) {
            if (this._layers[i][property] == id) {
                return this._layers[i];
            }
        }
    },
    leafRemoveLayer: function (id, property) {
        property = property || "id";
        const layer = this.leafGetLayer(id, property);
        if (layer)
            return layer.remove();
        return layer;
    },
    leafAddLayerToMap: function (id, property) {
        property = property || "id";
        const layer = this.leafGetLayer(id, property);
        if (layer)
            return layer.leafAddToMap();
        return layer;
    },
    leafRemoveAllInnerLayers: function () {
        this.eachLayer(function (layer) {
            layer.remove();
        });
        return this;
    },
    leafAddAllLayers: function () {
        this.eachLayer(function (layer) {
            layer.leafAddToMap();
        });
        return this;
    }
});

/*********************************************************
  Extending the leaflet library - End
**********************************************************/