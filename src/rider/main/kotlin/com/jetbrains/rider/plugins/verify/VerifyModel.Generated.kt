@file:Suppress("EXPERIMENTAL_API_USAGE","EXPERIMENTAL_UNSIGNED_LITERALS","PackageDirectoryMismatch","UnusedImport","unused","LocalVariableName","CanBeVal","PropertyName","EnumEntryName","ClassName","ObjectPropertyName","UnnecessaryVariable","SpellCheckingInspection")
package com.jetbrains.rider.plugins.verify

import com.jetbrains.rd.framework.*
import com.jetbrains.rd.framework.base.*
import com.jetbrains.rd.framework.impl.*

import com.jetbrains.rd.util.lifetime.*
import com.jetbrains.rd.util.reactive.*
import com.jetbrains.rd.util.string.*
import com.jetbrains.rd.util.*
import kotlin.time.Duration
import kotlin.reflect.KClass
import kotlin.jvm.JvmStatic



/**
 * #### Generated from [VerifyModel.kt:10]
 */
class VerifyModel private constructor(
    private val _compare: RdSignal<CompareData>
) : RdExtBase() {
    //companion
    
    companion object : ISerializersOwner {
        
        override fun registerSerializersCore(serializers: ISerializers)  {
            val classLoader = javaClass.classLoader
            serializers.register(LazyCompanionMarshaller(RdId(540707090302080028), classLoader, "com.jetbrains.rider.plugins.verify.CompareData"))
        }
        
        
        
        
        
        const val serializationHash = -8460501441895752832L
        
    }
    override val serializersOwner: ISerializersOwner get() = VerifyModel
    override val serializationHash: Long get() = VerifyModel.serializationHash
    
    //fields
    val compare: ISignal<CompareData> get() = _compare
    //methods
    //initializer
    init {
        bindableChildren.add("compare" to _compare)
    }
    
    //secondary constructor
    internal constructor(
    ) : this(
        RdSignal<CompareData>(CompareData)
    )
    
    //equals trait
    //hash code trait
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("VerifyModel (")
        printer.indent {
            print("compare = "); _compare.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    override fun deepClone(): VerifyModel   {
        return VerifyModel(
            _compare.deepClonePolymorphic()
        )
    }
    //contexts
    //threading
    override val extThreading: ExtThreadingKind get() = ExtThreadingKind.Default
}
val com.jetbrains.rd.ide.model.Solution.verifyModel get() = getOrCreateExtension("verifyModel", ::VerifyModel)



/**
 * #### Generated from [VerifyModel.kt:15]
 */
data class CompareData (
    val testName: String,
    val receivedFile: String,
    val verifiedFile: String
) : IPrintable {
    //companion
    
    companion object : IMarshaller<CompareData> {
        override val _type: KClass<CompareData> = CompareData::class
        override val id: RdId get() = RdId(540707090302080028)
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): CompareData  {
            val testName = buffer.readString()
            val receivedFile = buffer.readString()
            val verifiedFile = buffer.readString()
            return CompareData(testName, receivedFile, verifiedFile)
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: CompareData)  {
            buffer.writeString(value.testName)
            buffer.writeString(value.receivedFile)
            buffer.writeString(value.verifiedFile)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as CompareData
        
        if (testName != other.testName) return false
        if (receivedFile != other.receivedFile) return false
        if (verifiedFile != other.verifiedFile) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + testName.hashCode()
        __r = __r*31 + receivedFile.hashCode()
        __r = __r*31 + verifiedFile.hashCode()
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("CompareData (")
        printer.indent {
            print("testName = "); testName.print(printer); println()
            print("receivedFile = "); receivedFile.print(printer); println()
            print("verifiedFile = "); verifiedFile.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    //contexts
    //threading
}
