function MonitorPoint() {
    this.x = 0;
    this.y = 0;
    this.value = 0;
    this.id;
}
function MonitorItem(height, width, xLen) {
    this.maxValue = 100;
    this.changeMaxValue = false;
    this.label = "";
    this.items = new Array();
    this.strokeStyle = "chartreuse";
    this.height = height;
    this.width = width;
    this.lastValue;
    this.xLen = xLen;
    this.tag;
    this.labelFormater = null;
    this.lineWidth = 1;
    wlen = width / this.xLen;
    for (i = 0; i <= this.xLen; i++) {
        var p = new MonitorPoint();
        p.x = i * wlen;
        p.y = this.height;
        this.items.push(p);
    }

}
MonitorItem.prototype.getLabel = function () {
    if (this.labelFormater)
        return this.labelFormater(this);
    return this.label + " " + this.lastValue + '/' + this.maxValue
}
MonitorItem.prototype.reset = function () {
    this.items.clear();
    for (i = 0; i <= this.xLen; i++) {
        var p = new MonitorPoint();
        p.x = i * wlen;
        p.y = this.height;
        this.items.push(p);
    }
}
MonitorItem.prototype.push = function (value) {
    var length = this.items.length - 1;
    for (i = 0; i < length; i++) {
        var l = this.items[i];
        var c = this.items[i + 1];
        if (c && l)
            l.y = c.y;
    }
    this.lastValue = value;
    var p = this.items[this.items.length - 1];
    if (value >= this.maxValue) {
        p.value = this.maxValue - 4;
    }
    else {
        p.value = value;
    }
    var pe = this.height / this.maxValue;
    p.y = this.height - (p.value * pe);
    if (p.y < 2)
        p.y = 2;

}

MonitorItem.prototype.draw = function (context) {
    context.beginPath();
    var first = this.items[0];
    context.lineWidth = this.lineWidth;
    context.moveTo(first.x, first.y);
    for (i = 0; i < this.items.length; i++) {
        var p = this.items[i];
        context.lineTo(p.x, p.y);
    }
    context.fillStyle = '';
    context.strokeStyle = this.strokeStyle;
    context.stroke();
}

function Monitor(canvas) {
    this.colors = [];
    this.canvas = document.getElementById(canvas);
    this.context = this.canvas.getContext('2d');
    this.items = new Array();
    this.displayLabel = true;
    this.colorIndex = 0;
    this.lineWidth = 1;
    this.xLen = 200;
    this.selectID;
    this.initColor();
}
Monitor.prototype.initColor = function () {
    this.colors = ["#4aff1f", "#a4fef8", "#ff8a8d", "#dc69fe", "#a1a3ff", "#01c1ff", "#01ffa2", "#ffdfc4", "#759aff",
        "#fff393", "#edadfc", "#ff7805", "#ff4646", "#e6e200", "#b2ffb8", "#ffffff"];
}
Monitor.prototype.draw = function () {
    this.clear();
    var _this = this;
    var _selectItem;
    this.items.forEach(function (v, i) {

        if (v.id && v.id == _this.selectID) {
            _selectItem = v;
        }
        else {
            v.lineWidth = 1;
            v.draw(_this.context);
        }
        if (_selectItem) {
            _selectItem.lineWidth = 2;
            _selectItem.draw(_this.context);
        }

    });
}

Monitor.prototype.reset = function () {
    for (var i = 0; i < this.items.length; i++) {
        if (this.items[i].id == id) {
            this.items[i].reset();
        }
    }
}

Monitor.prototype.find = function (id) {
    for (var i = 0; i < this.items.length; i++) {
        if (this.items[i].id == id) {
            return this.items[i];
        }
    }
}

Monitor.prototype.removeAll = function () {
    this.items.clear();
    this.initColor();
}

Monitor.prototype.remove = function (id) {
    for (var i = 0; i < this.items.length; i++) {
        if (this.items[i].id == id) {
            this.colors.push(this.items[i].strokeStyle);
            var item = this.items.splice(i, 1);
            return item;
        }
    }
}
Monitor.prototype.create = function (id) {
    var item = new MonitorItem(this.canvas.height, this.canvas.width, this.xLen);
    item.id = id;
    item.lineWidth = this.lineWidth;
    var index = this.colorIndex % this.colors.length;
    this.colorIndex++;
    item.strokeStyle = this.colors.shift();
    this.items.push(item);
    return item;
}
Monitor.prototype.clear = function () {
    this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
    this.context.fillStyle = "#000000";
    this.context.fillRect(0, 0, this.canvas.width, this.canvas.height);
    var y = 0;
    while (y < this.canvas.height) {
        this.context.beginPath();
        this.context.moveTo(0, y);
        this.context.lineTo(this.canvas.width, y);
        this.context.lineWidth = 1;

        // set line color
        this.context.strokeStyle = '#187c00';
        this.context.stroke();
        y += 10;
    }
    var x = 0;
    while (x < this.canvas.width) {
        this.context.beginPath();
        this.context.moveTo(x, 0);
        this.context.lineTo(x, this.canvas.height);
        this.context.lineWidth = 1;
        // set line color
        this.context.strokeStyle = '#187c00';
        this.context.stroke();
        x += 10;
    }
    var _ths = this;
    if (this.displayLabel) {
        this.items.forEach(function (v, i) {
            _ths.context.font = '10pt Sans-serif';
            //if (v.lastValue > v.maxValue)
            //    _ths.context.fillStyle = 'orangered';
            //else
            _ths.context.fillStyle = v.strokeStyle
            _ths.context.fillText(v.getLabel(), 10, i * 16 + 16);
        });
    }

}
