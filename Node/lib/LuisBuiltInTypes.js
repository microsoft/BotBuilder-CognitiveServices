var BuiltInTypes = {
    Age: 'builtin.age',
    Dimension: 'builtin.dimension',
    Email: 'builtin.email',
    Money: 'builtin.money',
    Number: 'builtin.number',
    Ordinal: 'builtin.ordinal',
    Percentage: 'builtin.percentage',
    PhoneNumber: 'builtin.phonenumber',
    Temperature: 'builtin.temperature',
    Url: 'builtin.url',
    DateTime: {
        Date: 'builtin.datetime.date',
        Time: 'builtin.datetime.time',
        Duration: 'builtin.datetime.duration',
        Set: 'builtin.datetime.set'
    },
    DateTimeV2: {
        Date: 'builtin.datetimeV2.date',
        Time: 'builtin.datetimeV2.time',
        DateRange: 'builtin.datetimeV2.daterange',
        TimeRange: 'builtin.datetimeV2.timerange',
        DateTimeRange: 'builtin.datetimeV2.datetimerange',
        Duration: 'builtin.datetimeV2.duration',
        Set: 'builtin.datetimeV2.set'
    },
    Encyclopedia: {
        Person: 'builtin.encyclopedia.people.person',
        Organization: 'builtin.encyclopedia.organization.organization',
        Event: 'builtin.encyclopedia.time.event'
    },
    Geography: {
        City: 'builtin.geography.city',
        Country: 'builtin.geography.country',
        PointOfInterest: 'builtin.geography.pointOfInterest'
    }
};
module.exports = BuiltInTypes;
