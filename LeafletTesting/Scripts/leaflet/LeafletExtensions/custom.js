
/**********************************************************
    Extending the leaflet library - Start
**********************************************************/

L.Layer.include({
    omsAddToMap: function () {
        this.addTo(getMap());
    }
});

L.LayerGroup.include({
    omsGetLayer: function (id, property) {
        property = property || "id";
        for (var i in this._layers) {
            if (this._layers[i][property] == id) {
                return this._layers[i];
            }
        }
    },
    omsRemoveLayer: function (id, property) {
        property = property || "id";
        const layer = this.omsGetLayer(id, property);
        if (layer)
            layer.remove();
    },
    omsAddLayerToMap: function (id, property) {
        property = property || "id";
        const layer = this.omsGetLayer(id, property);
        if (layer)
            layer.omsAddToMap();
    },
    omsRemoveAllInnerLayers: function () {
        this.eachLayer(function (layer) {
            layer.remove();
        });
    },
    omsAddAllLayers: function () {
        this.eachLayer(function (layer) {
            layer.omsAddToMap();
        });
    }
});

      /*********************************************************
        Extending the leaflet library - End
    **********************************************************/